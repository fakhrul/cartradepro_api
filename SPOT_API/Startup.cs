using System.Text;
using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using SPOT_API.Authentication;
using SPOT_API.Persistence;
using SPOT_API.Extensions;
using SPOT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SPOT_API
{
    public class Startup
    {
        IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("WWW-Authenticate", "Pagination")
                        .WithOrigins(
                            "http://localhost:8080",
                            "http://localhost:8081", 
                            "http://localhost:3000",
                            "https://localhost:44305",
                            "http://128.199.166.113",
                            "http://192.168.0.180:8080",
                            "http://v2.thefibreguy.com",
                            "https://v2.thefibreguy.com",
                            "http://178.128.105.21:8102",
                            "http://cartradepro.safa.com.my",
                            "https://cartradepro.safa.com.my"
                        );
                });
            });

            services.AddDbContext<SpotDBContext>(opt =>
                 opt.UseNpgsql(_config.GetConnectionString("DB")
                //opt.UseSqlServer(
                //    _config.GetConnectionString("SpotDB2")
                ));

            services.AddIdentityServices(_config);
            // For Identity  
            /*services.AddIdentity<UsersAuth, IdentityRole>()
                .AddEntityFrameworkStores<SpotDBContext>()
                .AddDefaultTokenProviders();*/

            // Adding Authentication  
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //});

            // Adding Jwt Bearer  
            //.AddJwtBearer(options =>
            // {
            //     options.SaveToken = true;
            //     options.RequireHttpsMetadata = false;
            //     options.TokenValidationParameters = new TokenValidationParameters()
            //     {
            //         ValidateIssuer = true,
            //         ValidateAudience = true,
            //         ValidAudience = Configuration["JWT:ValidAudience"],
            //         ValidIssuer = Configuration["JWT:ValidIssuer"],
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
            //     };
            // });
            //services.AddControllersWithViews();
            services.AddControllers(opt =>
           {
               //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
               //opt.Filters.Add(new AuthorizeFilter(policy));
           }
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SPOT API", Version = "v1" });
            });

            services.AddApplicationServices(_config);

            // In production, the React files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/build";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Make sure you call this before calling app.UseMvc()
            //app.UseCors(
            //    options => options.WithOrigins("http://localhost:3000").AllowAnyMethod()
            //);


            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SPOT API v1"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SPOT API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseSpaStaticFiles();
            //app.UseApiResponseAndExceptionWrapper();
            app.UseApiResponseAndExceptionWrapper(
                new AutoWrapperOptions
                {
                    ShowApiVersion = true,
                    ApiVersion = "2.0",
                    ExcludePaths = new AutoWrapperExcludePath[] {
                        // Strict Match
                        new AutoWrapperExcludePath("/api/Areas/DownloadFileTemplate", ExcludeMode.Strict),
                        new AutoWrapperExcludePath("/api/Locations/FloorPlan", ExcludeMode.StartWith),
                        new AutoWrapperExcludePath("/api/Documents/Image", ExcludeMode.StartWith),
                        new AutoWrapperExcludePath("/api/Documents/File", ExcludeMode.StartWith),
                        new AutoWrapperExcludePath("/Hello", ExcludeMode.StartWith),
                        new AutoWrapperExcludePath("/api/FingerPrints/DownloadTemplate", ExcludeMode.StartWith),
                        new AutoWrapperExcludePath("/api/Stocks/bulk-import/template", ExcludeMode.StartWith),
                        
                        // StartWith
                        //new AutoWrapperExcludePaths("/dapr", ExcludeMode.StartWith),
                        //// Regex
                        //new AutoWrapperExcludePaths("/notice/.*|/notice", ExcludeMode.Regex)
                    }
                });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});
        }
    }
}
