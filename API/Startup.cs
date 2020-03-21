using System;
using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using Application.Profiles;
using Application.User;
using Application.Users;
using API.Middleware;
using API.SignalR;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // public void ConfigureDevelopmentServices (IServiceCollection services) {
        //     services.AddDbContext<DataContext> (opt => {
        //         opt.UseLazyLoadingProxies ();
        //         opt.UseSqlite (Configuration.GetConnectionString ("DefaultConnection"));
        //     });

        //     ConfigureServices (services);
        // }

        // public void ConfigureProductionServices (IServiceCollection services) {
        //     services.AddDbContext<DataContext> (opt => {
        //         opt.UseLazyLoadingProxies ();
        //         opt.UseMySql (Configuration.GetConnectionString ("DefaultConnection"));
        //     });

        //     ConfigureServices (services);
        // }

        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<DataContext> (opt => {
                opt.UseLazyLoadingProxies ();
                // opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                // opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                opt.UseMySql (Configuration.GetConnectionString ("DefaultConnection"));
            });

            // added cors configuration
            services.AddCors (opt => {
                opt.AddPolicy ("CorsPolicy",
                    policy => {
                        policy
                            .AllowAnyHeader ()
                            .AllowAnyMethod ()
                            .WithExposedHeaders ("WWW-Authenticate")
                            .WithOrigins ("http://localhost:3000")
                            .AllowCredentials ();
                    });
            });

            // configured mediatR
            services.AddMediatR (typeof (List.Handler).Assembly);
            services.AddMediatR (typeof (Login.Handler).Assembly);
            services.AddMediatR (typeof (Register.Handler).Assembly);
            services.AddMediatR (typeof (Attend.Handler).Assembly);
            services.AddMediatR (typeof (SetMain.Handler).Assembly);
            services.AddSignalR ();
            services.AddAutoMapper (typeof (List.Handler));

            // configured fluent API
            services.AddControllers (opt => {
                    var policy = new AuthorizationPolicyBuilder ().RequireAuthenticatedUser ().Build ();
                    opt.Filters.Add (new AuthorizeFilter (policy));
                })
                .AddFluentValidation (cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Create> (); });

            // setting up IDentity
            var builder = services.AddIdentityCore<AppUser> ();
            var identityBuilder = new IdentityBuilder (builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext> ();
            identityBuilder.AddSignInManager<SignInManager<AppUser>> ();
            // services.TryAddSingleton<ISystemClock, SystemClock>();

            services.AddAuthorization (opt => {
                opt.AddPolicy ("IsActivityHost", policy => { policy.Requirements.Add (new IsHostRequirement ()); });
            });

            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler> ();

            var key = new SymmetricSecurityKey (
                Encoding.UTF8.GetBytes (Configuration.GetSection ("AppSettings:TokenKey").Value));

            // configuration of Jwt Authentication
            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme).AddJwtBearer (opt => {
                opt.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                };
                opt.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        // checking that request contain access token and path contain - /chat
                        if (!string.IsNullOrEmpty (accessToken) && (path.StartsWithSegments ("/chat"))) {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IJwtGenerator, JwtGenerator> ();
            services.AddScoped<IUserAccessor, UserAccessor> ();
            services.AddScoped<IProfileReader, ProfileReader> ();
            services.Configure<CloudinarySettings> (Configuration.GetSection ("CloudinarySettings"));
            services.AddScoped<IPhotoAccessor, PhotoAccessor> ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseMiddleware<ErrorHandlingMiddleware> ();
            if (env.IsDevelopment ()) {
                // app.UseDeveloperExceptionPage();
            } else {
                // app.UseHsts();
            }

            /* SECURITY START */
            app.UseXContentTypeOptions (); // prevent content sniffing
            app.UseReferrerPolicy (opt =>
                opt.NoReferrer ()); // restrict the amount of information to other site when referring to other site
            app.UseXXssProtection (opt =>
                opt.EnabledWithBlockMode ()); // this one stops the pages to loading when they detect cross site scripting
            app.UseXfo (opt => opt.Deny ()); // prevent click jack attacks
            // below is the content security policy
            // opt.BlockAllMixedContent()) -> this one loading any assets using http when the page is loaded as https // UseCspReportOnly¸
            app.UseCsp (opt => opt
                .BlockAllMixedContent ()
                .StyleSources (s => s.Self ().CustomSources ("https://fonts.googleapis.com",
                    "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
                .FontSources (s => s.Self ().CustomSources ("https://fonts.gstatic.com", "data:"))
                .FormActions (s => s.Self ())
                .FrameAncestors (s => s.Self ())
                .ImageSources (s => s.Self ().CustomSources ("https://res.cloudinary.com", "blob:"))
                .ScriptSources (s => s.Self ().CustomSources ("sha256-ma5XxS1EBgt17N22Qq31rOxxRWRfzUTQS1KOtfYwuNo="))
            );

            /* SECURITY END */

            app.UseHttpsRedirection ();
            app.UseDefaultFiles ();
            app.UseStaticFiles ();
            app.UseRouting ();
            app.UseAuthentication ();
            app.UseAuthorization ();
            app.UseCors ("CorsPolicy");
            // app.UseMvc();
            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
                endpoints.MapHub<ChatHub> ("/chat");
                endpoints.MapFallbackToController ("Index", "Fallback");
            });
        }
    }
}