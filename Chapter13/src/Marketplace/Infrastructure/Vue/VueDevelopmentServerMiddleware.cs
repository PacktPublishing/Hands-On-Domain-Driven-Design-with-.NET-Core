// This file is based on the code from the ASP.NET Core repository
// https://github.com/aspnet/AspNetCore/tree/master/src/Middleware/SpaServices.Extensions/src
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Marketplace.Infrastructure.Vue
{
    /// <summary>
    ///     Extension methods for enabling React development server middleware support.
    /// </summary>
    public static class VueDevelopmentServerMiddlewareExtensions
    {
        /// <summary>
        ///     Handles requests by passing them through to an instance of the create-react-app server.
        ///     This means you can always serve up-to-date CLI-built resources without having
        ///     to run the create-react-app server manually.
        ///     This feature should only be used in development. For production deployments, be
        ///     sure not to enable the create-react-app server.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder" />.</param>
        /// <param name="npmScript">The name of the script in your package.json file that launches the create-react-app server.</param>
        public static void UseVueDevelopmentServer(
            this ISpaBuilder spaBuilder,
            string npmScript)
        {
            if (spaBuilder == null) throw new ArgumentNullException(nameof(spaBuilder));

            var spaOptions = spaBuilder.Options;

            if (string.IsNullOrEmpty(spaOptions.SourcePath))
                throw new InvalidOperationException(
                    $"To use {nameof(UseVueDevelopmentServer)}, you must supply a non-empty value for the {nameof(SpaOptions.SourcePath)} property of {nameof(SpaOptions)} when calling {nameof(SpaApplicationBuilderExtensions.UseSpa)}."
                );

            VueDevelopmentServerMiddleware.Attach(spaBuilder, npmScript);
        }
    }

    internal static class VueDevelopmentServerMiddleware
    {
        const string LogCategoryName = "Microsoft.AspNetCore.SpaServices";

        static readonly TimeSpan
            RegexMatchTimeout =
                TimeSpan.FromSeconds(5); // This is a development-time only feature, so a very long timeout is fine

        public static void Attach(
            ISpaBuilder spaBuilder,
            string npmScriptName)
        {
            var sourcePath = spaBuilder.Options.SourcePath;
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("Cannot be null or empty", nameof(sourcePath));

            if (string.IsNullOrEmpty(npmScriptName)) throw new ArgumentException("Cannot be null or empty", nameof(npmScriptName));

            // Start create-react-app and attach to middleware pipeline
            var appBuilder = spaBuilder.ApplicationBuilder;
            var logger = LoggerFinder.GetOrCreateLogger(appBuilder, LogCategoryName);
            var portTask = StartCreateVueAppServerAsync(sourcePath, npmScriptName, logger);

            // Everything we proxy is hardcoded to target http://localhost because:
            // - the requests are always from the local machine (we're not accepting remote
            //   requests that go directly to the create-react-app server)
            // - given that, there's no reason to use https, and we couldn't even if we
            //   wanted to, because in general the create-react-app server has no certificate
            var targetUriTask = portTask.ContinueWith(
                task => new UriBuilder("http", "localhost", task.Result).Uri
            );

            spaBuilder.UseProxyToSpaDevelopmentServer(
                () =>
                {
                    // On each request, we create a separate startup task with its own timeout. That way, even if
                    // the first request times out, subsequent requests could still work.
                    var timeout = spaBuilder.Options.StartupTimeout;

                    return targetUriTask.WithTimeout(
                        timeout,
                        "The Vue development server did not start listening for requests " +
                        $"within the timeout period of {timeout.Seconds} seconds. " +
                        "Check the log output for error information."
                    );
                }
            );
        }

        static async Task<int> StartCreateVueAppServerAsync(
            string sourcePath,
            string npmScriptName,
            ILogger logger)
        {
            var portNumber = TcpPortFinder.FindAvailablePort();
            logger.LogInformation($"Starting Vue development server on port {portNumber}...");

            var envVars = new Dictionary<string, string>
            {
                {"PORT", portNumber.ToString()},
                {
                    "BROWSER", "none"
                } // We don't want create-react-app to open its own extra browser window pointing to the internal dev server port
            };

            var npmScriptRunner = new NpmScriptRunner(
                sourcePath, npmScriptName, null, envVars
            );
            npmScriptRunner.AttachToLogger(logger);

            using (var stdErrReader = new EventedStreamStringReader(npmScriptRunner.StdErr))
            {
                try
                {
                    // Although the React dev server may eventually tell us the URL it's listening on,
                    // it doesn't do so until it's finished compiling, and even then only if there were
                    // no compiler warnings. So instead of waiting for that, consider it ready as soon
                    // as it starts listening for requests.
                    await npmScriptRunner.StdOut.WaitForMatch(
                        new Regex("App running at", RegexOptions.None, RegexMatchTimeout)
                    );
                }
                catch (EndOfStreamException ex)
                {
                    throw new InvalidOperationException(
                        $"The NPM script '{npmScriptName}' exited without indicating that the " +
                        "Vue development server was listening for requests. The error output was: " +
                        $"{stdErrReader.ReadAsString()}", ex
                    );
                }
            }

            return portNumber;
        }
    }

    internal static class LoggerFinder
    {
        public static ILogger GetOrCreateLogger(
            IApplicationBuilder appBuilder,
            string logCategoryName)
        {
            // If the DI system gives us a logger, use it. Otherwise, set up a default one
            var loggerFactory = appBuilder.ApplicationServices.GetService<ILoggerFactory>();

            var logger = loggerFactory != null
                ? loggerFactory.CreateLogger(logCategoryName)
                : NullLogger.Instance;
            return logger;
        }
    }

    internal static class TcpPortFinder
    {
        public static int FindAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            try
            {
                return ((IPEndPoint) listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }

    /// <summary>
    ///     Captures the completed-line notifications from a <see cref="EventedStreamReader" />,
    ///     combining the data into a single <see cref="string" />.
    /// </summary>
    internal class EventedStreamStringReader : IDisposable
    {
        readonly EventedStreamReader _eventedStreamReader;
        bool _isDisposed;
        readonly StringBuilder _stringBuilder = new StringBuilder();

        public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
        {
            _eventedStreamReader = eventedStreamReader
                                   ?? throw new ArgumentNullException(nameof(eventedStreamReader));
            _eventedStreamReader.OnReceivedLine += OnReceivedLine;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
                _isDisposed = true;
            }
        }

        public string ReadAsString() => _stringBuilder.ToString();

        void OnReceivedLine(string line) => _stringBuilder.AppendLine(line);
    }

    /// <summary>
    ///     Wraps a <see cref="StreamReader" /> to expose an evented API, issuing notifications
    ///     when the stream emits partial lines, completed lines, or finally closes.
    /// </summary>
    internal class EventedStreamReader
    {
        public delegate void OnReceivedChunkHandler(ArraySegment<char> chunk);

        public delegate void OnReceivedLineHandler(string line);

        public delegate void OnStreamClosedHandler();

        readonly StringBuilder _linesBuffer;

        readonly StreamReader _streamReader;

        public EventedStreamReader(StreamReader streamReader)
        {
            _streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
            _linesBuffer = new StringBuilder();
            Task.Factory.StartNew(Run);
        }

        public event OnReceivedChunkHandler OnReceivedChunk;
        public event OnReceivedLineHandler OnReceivedLine;
        public event OnStreamClosedHandler OnStreamClosed;

        public Task<Match> WaitForMatch(Regex regex)
        {
            var tcs = new TaskCompletionSource<Match>();
            var completionLock = new object();

            OnReceivedLineHandler onReceivedLineHandler = null;
            OnStreamClosedHandler onStreamClosedHandler = null;

            void ResolveIfStillPending(Action applyResolution)
            {
                lock (completionLock)
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        OnReceivedLine -= onReceivedLineHandler;
                        OnStreamClosed -= onStreamClosedHandler;
                        applyResolution();
                    }
                }
            }

            onReceivedLineHandler = line =>
            {
                var match = regex.Match(line);
                if (match.Success) ResolveIfStillPending(() => tcs.SetResult(match));
            };

            onStreamClosedHandler = () => { ResolveIfStillPending(() => tcs.SetException(new EndOfStreamException())); };

            OnReceivedLine += onReceivedLineHandler;
            OnStreamClosed += onStreamClosedHandler;

            return tcs.Task;
        }

        async Task Run()
        {
            var buf = new char[8 * 1024];

            while (true)
            {
                var chunkLength = await _streamReader.ReadAsync(buf, 0, buf.Length);

                if (chunkLength == 0)
                {
                    OnClosed();
                    break;
                }

                OnChunk(new ArraySegment<char>(buf, 0, chunkLength));

                var lineBreakPos = Array.IndexOf(buf, '\n', 0, chunkLength);

                if (lineBreakPos < 0)
                {
                    _linesBuffer.Append(buf, 0, chunkLength);
                }
                else
                {
                    _linesBuffer.Append(buf, 0, lineBreakPos + 1);
                    OnCompleteLine(_linesBuffer.ToString());
                    _linesBuffer.Clear();
                    _linesBuffer.Append(buf, lineBreakPos + 1, chunkLength - (lineBreakPos + 1));
                }
            }
        }

        void OnChunk(ArraySegment<char> chunk)
        {
            var dlg = OnReceivedChunk;
            dlg?.Invoke(chunk);
        }

        void OnCompleteLine(string line)
        {
            var dlg = OnReceivedLine;
            dlg?.Invoke(line);
        }

        void OnClosed()
        {
            var dlg = OnStreamClosed;
            dlg?.Invoke();
        }
    }

    internal static class TaskTimeoutExtensions
    {
        public static async Task WithTimeout(this Task task, TimeSpan timeoutDelay, string message)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeoutDelay)))
                task.Wait(); // Allow any errors to propagate
            else
                throw new TimeoutException(message);
        }

        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeoutDelay, string message)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeoutDelay)))
                return task.Result;

            throw new TimeoutException(message);
        }
    }

    /// <summary>
    ///     Executes the <c>script</c> entries defined in a <c>package.json</c> file,
    ///     capturing any output written to stdio.
    /// </summary>
    internal class NpmScriptRunner
    {
        static readonly Regex AnsiColorRegex =
            new Regex("\x001b\\[[0-9;]*m", RegexOptions.None, TimeSpan.FromSeconds(1));

        public NpmScriptRunner(
            string workingDirectory,
            string scriptName,
            string arguments,
            IDictionary<string, string> envVars)
        {
            if (string.IsNullOrEmpty(workingDirectory)) throw new ArgumentException("Cannot be null or empty.", nameof(workingDirectory));

            if (string.IsNullOrEmpty(scriptName)) throw new ArgumentException("Cannot be null or empty.", nameof(scriptName));

            var npmExe = "npm";
            var completeArguments = $"run {scriptName} -- {arguments ?? string.Empty}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, the NPM executable is a .cmd file, so it can't be executed
                // directly (except with UseShellExecute=true, but that's no good, because
                // it prevents capturing stdio). So we need to invoke it via "cmd /c".
                npmExe = "cmd";
                completeArguments = $"/c npm {completeArguments}";
            }

            var processStartInfo = new ProcessStartInfo(npmExe)
            {
                Arguments = completeArguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };

            if (envVars != null)
                foreach (var keyValuePair in envVars)
                    processStartInfo.Environment[keyValuePair.Key] = keyValuePair.Value;

            var process = LaunchNodeProcess(processStartInfo);
            StdOut = new EventedStreamReader(process.StandardOutput);
            StdErr = new EventedStreamReader(process.StandardError);
        }

        public EventedStreamReader StdOut { get; }
        public EventedStreamReader StdErr { get; }

        public void AttachToLogger(ILogger logger)
        {
            // When the NPM task emits complete lines, pass them through to the real logger
            StdOut.OnReceivedLine += line =>
            {
                if (!string.IsNullOrWhiteSpace(line)) logger.LogInformation(StripAnsiColors(line));
            };

            StdErr.OnReceivedLine += line =>
            {
                if (!string.IsNullOrWhiteSpace(line)) logger.LogError(StripAnsiColors(line));
            };

            // But when it emits incomplete lines, assume this is progress information and
            // hence just pass it through to StdOut regardless of logger config.
            StdErr.OnReceivedChunk += chunk =>
            {
                var containsNewline = Array.IndexOf(
                                          chunk.Array, '\n', chunk.Offset, chunk.Count
                                      ) >= 0;
                if (!containsNewline) Console.Write(chunk.Array, chunk.Offset, chunk.Count);
            };
        }

        static string StripAnsiColors(string line) => AnsiColorRegex.Replace(line, string.Empty);

        static Process LaunchNodeProcess(ProcessStartInfo startInfo)
        {
            try
            {
                var process = Process.Start(startInfo);

                // See equivalent comment in OutOfProcessNodeInstance.cs for why
                process.EnableRaisingEvents = true;

                return process;
            }
            catch (Exception ex)
            {
                var message = "Failed to start 'npm'. To resolve this:.\n\n"
                              + "[1] Ensure that 'npm' is installed and can be found in one of the PATH directories.\n"
                              + $"    Current PATH enviroment variable is: {Environment.GetEnvironmentVariable("PATH")}\n"
                              + "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
                              + "[2] See the InnerException for further details of the cause.";
                throw new InvalidOperationException(message, ex);
            }
        }
    }
}