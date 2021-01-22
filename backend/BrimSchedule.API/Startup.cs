using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BrimSchedule.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(b => b.AddPolicy("CorsPolicy", builder =>
            {
                builder.SetIsOriginAllowed(s => true);
                builder.AllowAnyHeader();
                builder.AllowCredentials();
                builder.AllowAnyMethod();
            }));
            
            // TODO Uncomment and fill in correspond classes when EF will be set up
            // services.AddDbContext<T>(opt => opt.UseSqlServer(Configuration.GetConnectionString(("Default")), 
            //     b => b.MigrationsAssembly("BrimSchedule.Persistence")));

            // services.AddIdentity<User, IdentityRole>()
            //    .AddEntityFrameworkStores<SparkleDbContext>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => {
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var authorization = context.Request.Headers["Authorization"].ToString();

                        if (string.IsNullOrEmpty(authorization))
                        {
                            context.NoResult();
                            return;
                        }

                        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return;
                        
                        var token = authorization.Substring("Bearer ".Length).Trim();

                        if (string.IsNullOrEmpty(token))
                        {
                            context.NoResult();
                            return;
                        }

                        var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                        context.Principal = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                decodedToken.Claims.Select(c => new Claim(c.Key, c.Value.ToString()))
                                    .Append(new Claim(ClaimsIdentity.DefaultNameClaimType,  decodedToken.Uid)), 
                                JwtBearerDefaults.AuthenticationScheme
                            ));

                        context.Success();
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Fail(context.Exception);
                        return Task.CompletedTask;
                    }
                };
            });
            
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
            
                    options.SubstituteApiVersionInUrl = true;
                });
            
            
            services.AddSwaggerGen(
                options =>
                {
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                        {
                            Title =
                                $"{GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product}",
                            Version = description.ApiVersion.ToString(),
                            Description = description.IsDeprecated ? "DEPRECATED" : ""
            
                        });
                    }
            
                    // TODO Figure out is it necessary
                    //options.OperationFilter<SwaggerDefaultValues>();
            
                    options.IncludeXmlComments(Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        $"{GetType().Assembly.GetName().Name}.xml"
                    ));
            
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Description = "Введите в поле JWT токен с припиской Bearer",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
            
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme()
                            {
                                Name = "Bearer"
                            },
                            new List<string>()
                        }
                    });
            
                });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
                
            app.UseRouting();

            app.UseCors("CorsPolicy");
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
            
            app.UseHttpsRedirection();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var alias = string.IsNullOrWhiteSpace(Configuration["Swagger:Alias"]) ? "" : $"/{Configuration["Swagger:Alias"]}";
            
                        options.SwaggerEndpoint($"{alias}/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}