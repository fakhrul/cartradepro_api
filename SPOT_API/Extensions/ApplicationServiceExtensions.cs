using Application.Interfaces;
using Azure.Storage.Blobs;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            //});
            //services.AddDbContext<DataContext>(options =>
            //{
            //    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //    string connStr;

            //    // Depending on if in development or production, use either Heroku-provided
            //    // connection string, or development connection string from env var.
            //    if (env == "Development")
            //    {
            //        // Use connection string from file.
            //        connStr = config.GetConnectionString("DefaultConnection");
            //    }
            //    else
            //    {
            //        // Use connection string provided at runtime by Heroku.
            //        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            //        // Parse connection URL to connection string for Npgsql
            //        connUrl = connUrl.Replace("postgres://", string.Empty);
            //        var pgUserPass = connUrl.Split("@")[0];
            //        var pgHostPortDb = connUrl.Split("@")[1];
            //        var pgHostPort = pgHostPortDb.Split("/")[0];
            //        var pgDb = pgHostPortDb.Split("/")[1];
            //        var pgUser = pgUserPass.Split(":")[0];
            //        var pgPass = pgUserPass.Split(":")[1];
            //        var pgHost = pgHostPort.Split(":")[0];
            //        var pgPort = pgHostPort.Split(":")[1];

            //        connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
            //    }

            //    // Whether the connection string came from the local development configuration file
            //    // or from the environment variable from Heroku, use it to set up your DbContext.
            //    options.UseNpgsql(connStr);
            //});

            //services.AddCors(opt =>
            //{
            //    opt.AddPolicy("CorsPolicy", policy =>
            //    {
            //        policy
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .AllowCredentials()
            //            .WithExposedHeaders("WWW-Authenticate", "Pagination")
            //            .WithOrigins("http://localhost:8080");
            //    });
            //});
            //services.AddMediatR(typeof(List.Handler).Assembly);
            //services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddScoped<IUserAccessor, UserAccessor>();

            services.AddScoped(_ =>
            {
                return new BlobServiceClient(config.GetConnectionString("SpotFileStore"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;
            });

            //services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            //services.AddScoped<EmailSender>();
            //services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            //services.AddSignalR();
            //services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => {
            //    apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext => {
            //        var pd = new ProblemDetails();
            //        pd.Type = apiBehaviorOptions.ClientErrorMapping[400].Link;
            //        pd.Title = apiBehaviorOptions.ClientErrorMapping[400].Title;
            //        pd.Status = 400;
            //        pd.Extensions.Add("traceId", actionContext.HttpContext.TraceIdentifier);
            //        return new BadRequestObjectResult(pd);
            //    };
            //});


            return services;
        }
    }
}
