﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApp.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var connectionString = @"Data Source=C:\Projeto.WebIdentity\WebApp.Identity\IdentityCurso.db";
            var migrationAssembly = typeof(Startup)
                .GetTypeInfo().Assembly
                .GetName().Name;

            services.AddDbContext<MyUserDbContext>(
                opt => opt.UseSqlite(connectionString, sql =>
                sql.MigrationsAssembly(migrationAssembly))
                );

            services.AddIdentity<MyUser, IdentityRole>(options => 
            {
                 options.SignIn.RequireConfirmedEmail = true;

                 options.Password.RequireDigit = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireUppercase = false;
                 options.Password.RequiredLength = 4;

                 options.Lockout.MaxFailedAccessAttempts = 3;
                 options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<MyUserDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<MyUser>,
                MyUserClaimsPrincipalFactory>();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

            //services.AddTransient<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
