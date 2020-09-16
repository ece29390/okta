using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace dm_14475
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
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;      //If cookies not provided => Ask OpenIdConnect
                   options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               })
                 .AddCookie(options => {

                     options.ExpireTimeSpan = TimeSpan.FromDays(1);

                     options.ForwardDefaultSelector = context =>
                      {
                       var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                       if (authHeader?.StartsWith("Bearer ") == true)  //Authentification on the APi from postman or consumer of the BO Api (not the ui front in react)
                        {
                           return JwtBearerDefaults.AuthenticationScheme;
                       }
                       return CookieAuthenticationDefaults.AuthenticationScheme; //Authentification cookies for React front and Api call, if not set it will ask to DefaultChallengeScheme witch is linked to : OpenIdConnectDefaults
                    };

                 })
               .AddOpenIdConnect(options =>
               {
                   options.ClientId = "0oaz03fm2u0MZWNYM4x6";
                   options.Authority = "https://dev-455423.okta.com/oauth2/default";
                   //options.CallbackPath = "/signin-oidc";

                   //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                   //options.RequireHttpsMetadata = true;
                   //options.ClientSecret = "8h9VRIneMFHJOqNuyk2FS1ap1WVyI29L_H7Ftis6";
                   //options.ResponseType = OpenIdConnectResponseType.Code;
                   //options.GetClaimsFromUserInfoEndpoint = true;
                   //options.Scope.Add("openid");
                   //options.Scope.Add("profile");
                   //options.SaveTokens = true;
                   //options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   //{
                   //    NameClaimType = "name"
                   //    ,
                   //    ValidateIssuer = true
                   //};


                })
                .AddJwtBearer(options =>

                {

                    options.Audience = "api://default";

                    options.Authority = "https://dev-455423.okta.com/oauth2/default";

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters

                    {

                        ClockSkew = TimeSpan.FromMinutes(10),

                        RequireSignedTokens = true,

                        RequireExpirationTime = true,

                        ValidateLifetime = true,

                        ValidateAudience = true,

                        ValidAudiences = new string[] { "api://default" },

                        ValidateIssuer = true,

                        ValidIssuers = new string[] { "https://dev-455423.okta.com/oauth2/default" }

                    };

                    options.RequireHttpsMetadata = false;

                });

            ;
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
