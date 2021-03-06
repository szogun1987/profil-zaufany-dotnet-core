﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfilZaufany.Sign;
using ProfilZaufany.TestApp.Helpers;
using ProfilZaufany.UserInfo;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp
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
            services.AddMvc();

            var certificateStore = new SecretsStore();
            services.AddSingleton<ISecretsStore>(certificateStore);
            services.AddSingleton<IX509Provider>(certificateStore);

            services.AddScoped<ISigningService>(provider => new SigningService(Environment.Test, provider.GetService<IX509Provider>()));
            services.AddScoped<IUserInfoService>(provider => new UserInfoService(Environment.Test, provider.GetService<IX509Provider>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
