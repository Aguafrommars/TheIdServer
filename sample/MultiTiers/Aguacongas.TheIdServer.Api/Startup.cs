using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Reflection;

namespace Aguacongas.TheIdServer.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString))
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>();

            services.Configure<SendGridOptions>(Configuration)
                .AddControllersWithViews(options =>
            {
                options.AddIdentityServerAdminFilters();
            })
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerAdmin();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(SharedConstants.WRITER, policy =>
                {
                    policy.RequireAssertion(context =>
                       context.User.IsInRole(SharedConstants.WRITER));
                });
                options.AddPolicy(SharedConstants.READER, policy =>
                {
                    policy.RequireAssertion(context =>
                       context.User.IsInRole(SharedConstants.READER));
                });
            })
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = "https://localhost:6443";
                    options.RequireHttpsMetadata = false;
                    options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
                    options.ApiName = "theidserveradminapi";
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                    options.LegacyAudienceValidation = true;
                })
                .AddDynamic<SchemeDefinition>()
                .AddEntityFrameworkStore<ConfigurationDbContext>()
                .AddGoogle()
                .AddFacebook()
                .AddOpenIdConnect()
                .AddTwitter()
                .AddMicrosoftAccount()
                .AddOAuth("OAuth", options =>
                {
                });


            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error")
                    .UseHsts();
            }

            app.UseSerilogRequestLogging()
                .UseHttpsRedirection()
                .UseResponseCompression()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(enpoints =>
                {
                    enpoints.MapAdminApiControllers();
                });
        }
    }
}
