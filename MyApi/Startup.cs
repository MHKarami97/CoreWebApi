using System;
using AspNetCoreRateLimit;
using Autofac;
using Common;
using WebFramework.Swagger;
using WebFramework.Middlewares;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OwaspHeaders.Core.Extensions;
using Services.Identity;

namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _siteSetting = Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.InitializeAutoMapper();

            services.AddDbContext(Configuration);

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.AddMinimalMvc();

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdminPolicy", policy =>
                    policy.RequireRole(Roles.Admin));
                options.AddPolicy("WorkerPolicy", policy =>
                    policy.RequireRole(Roles.Admin, Roles.Worker));
                options.AddPolicy("WriterPolicy", policy =>
                    policy.RequireRole(Roles.Admin, Roles.Writer));
                options.AddPolicy("MemberPolicy", policy =>
                    policy.RequireRole(Roles.Admin, Roles.Member, Roles.Worker));
            });

            services.AddMemoryCache();

            services.AddSwagger();

            services.AddOptions();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            //services.AddElmah(Configuration, _siteSetting);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacConfigurationExtensions());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();

            app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            app.UseHsts(env);

            //app.UseElmah();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSwaggerAndUi();

            app.UseRouting();

            app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareConfiguration.CustomConfiguration());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}