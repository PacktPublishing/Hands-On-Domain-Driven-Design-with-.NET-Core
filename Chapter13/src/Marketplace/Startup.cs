using EventStore.ClientAPI;
using Marketplace.Ads;
using Marketplace.EventStore;
using Marketplace.Infrastructure.Currency;
using Marketplace.Infrastructure.Profanity;
using Marketplace.Infrastructure.Vue;
using Marketplace.Modules.Images;
using Marketplace.PaidServices;
using Marketplace.Users;
using Marketplace.WebApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using static Marketplace.Infrastructure.RavenDb.Configuration;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

// ReSharper disable UnusedMember.Global

[assembly: ApiController]

namespace Marketplace
{
    public class Startup
    {
        public const string CookieScheme = "MarketplaceScheme";

        public Startup(
            IHostingEnvironment environment,
            IConfiguration configuration
        )
        {
            Environment = environment;
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var esConnection = EventStoreConnection.Create(
                Configuration["eventStore:connectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName
            );
            var purgomalumClient = new PurgomalumClient();

            var documentStore = ConfigureRavenDb(
                Configuration["ravenDb:server"]
            );

            services.AddSingleton(new ImageQueryService(ImageStorage.GetFile));
            services.AddSingleton(esConnection);
            services.AddSingleton(documentStore);

            services.AddSingleton<IHostedService, EventStoreService>();

            services
                .AddAuthentication(
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
                .AddCookie();

            services
                .AddMvcCore(
                    options => options.Conventions.Add(new CommandConvention())
                )
                .AddApplicationPart(GetType().Assembly)
                .AddAdsModule(
                    "ClassifiedAds",
                    new FixedCurrencyLookup(),
                    ImageStorage.UploadFile
                )
                .AddUsersModule(
                    "Users",
                    purgomalumClient.CheckForProfanity
                )
                .AddPaidServicesModule("PaidServices")
                .AddJsonFormatters()
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSpaStaticFiles(
                configuration =>
                    configuration.RootPath = "ClientApp/dist"
            );

            services.AddSwaggerGen(
                c =>
                    c.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "ClassifiedAds", Version = "v1"
                        }
                    )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        "default",
                        "{controller=Home}/{action=Index}/{id?}"
                    );

                    routes.MapRoute(
                        "api",
                        "api/{controller=Home}/{action=Index}/{id?}"
                    );

                    routes.MapSpaFallbackRoute(
                        "spa-fallback",
                        new {controller = "Home", action = "Index"}
                    );
                }
            );

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(
                c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json", "ClassifiedAds v1"
                )
            );

            app.UseSpa(
                spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                        spa.UseVueDevelopmentServer("serve:bs");
                }
            );
        }
    }
}