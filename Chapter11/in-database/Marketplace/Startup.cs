using System;
using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Swashbuckle.AspNetCore.Swagger;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

// ReSharper disable UnusedMember.Global

namespace Marketplace
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var esConnection = EventStoreConnection.Create(
                Configuration["eventStore:connectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);
            var store = new EsAggregateStore(esConnection);
            var purgomalumClient = new PurgomalumClient();
            var documentStore = ConfigureRavenDb(Configuration.GetSection("ravenDb"));

            Func<IAsyncDocumentSession> getSession = () => documentStore.OpenAsyncSession();

            services.AddTransient(c => getSession());

            services.AddSingleton(esConnection);
            services.AddSingleton<IAggregateStore>(store);

            services.AddSingleton(new ClassifiedAdsApplicationService(
                store, new FixedCurrencyLookup()));
            services.AddSingleton(new UserProfileApplicationService(
                store, t => purgomalumClient.CheckForProfanity(t)));

            var projectionManager = new ProjectionManager(esConnection,
                new RavenDbCheckpointStore(getSession, "readmodels"),
                new ClassifiedAdDetailsProjection(getSession,
                    async userId => (await getSession.GetUserDetails(userId))?.DisplayName),
                new ClassifiedAdUpcasters(esConnection,
                    async userId => (await getSession.GetUserDetails(userId))?.PhotoUrl),
                new UserDetailsProjection(getSession));

            services.AddSingleton<IHostedService>(
                new EventStoreService(esConnection, projectionManager));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "ClassifiedAds",
                        Version = "v1"
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1"));
        }

        private static IDocumentStore ConfigureRavenDb(IConfiguration configuration)
        {
            var store = new DocumentStore
            {
                Urls = new[] {configuration["server"]},
                Database = configuration["database"]
            };
            store.Initialize();
            var record = store.Maintenance.Server.Send(
                new GetDatabaseRecordOperation(store.Database));
            if (record == null)
            {
                store.Maintenance.Server.Send(
                    new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
            }

            return store;
        }
    }
}