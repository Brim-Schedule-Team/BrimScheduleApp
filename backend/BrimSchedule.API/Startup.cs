using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BrimSchedule.API.Config.SwaggerConfiguration;
using BrimSchedule.API.Services;
using BrimSchedule.API.Services.Authentication;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrimSchedule.API
{
	public class Startup
	{
		private IWebHostEnvironment CurrentEnvironment { get; }
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			CurrentEnvironment = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureDependencyInjection(CurrentEnvironment.IsDevelopment(), Configuration);
			services.AddControllers();
			services.AddHealthChecks();

			services.AddCors(b =>
			{
				b.AddPolicy("DevCorsPolicy", builder =>
				{
					builder.SetIsOriginAllowed(_ => true);
					builder.AllowAnyHeader();
					builder.AllowCredentials();
					builder.AllowAnyMethod();
				});

				b.AddDefaultPolicy(_ =>
				{

				});
			});

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opt => {
				opt.Events = new JwtBearerEvents
				{
					OnMessageReceived = async context => await AuthTokenHandler.HandleTokenAsync(context),
					OnAuthenticationFailed = context =>
					{
						context.Fail(context.Exception);
						return Task.CompletedTask;
					}
				};
			});

			services.AddAuthorization();

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

			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
			services.AddSwaggerGen(
				options =>
				{
					// necessary for correct UI of different api versions
					// add a custom operation filter which sets default values
					options.OperationFilter<SwaggerDefaultValues>();

					options.IncludeXmlComments(Path.Combine(
						Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
						$"{GetType().Assembly.GetName().Name}.xml"
					));

					options.AddSecurityDefinition(AuthTokenHandler.Bearer, new OpenApiSecurityScheme
					{
						In = ParameterLocation.Header,
						Description = $"Введите в поле JWT токен с префиксом {AuthTokenHandler.Bearer}",
						Name = AuthTokenHandler.HeaderName,
						Type = SecuritySchemeType.ApiKey,
						Scheme = AuthTokenHandler.Bearer
					});

					options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Name = AuthTokenHandler.Bearer,
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = AuthTokenHandler.Bearer
								},
								Scheme = "oauth2",
								In = ParameterLocation.Header,
							},
							new List<string>()
						}
					});
				});

			services.AddControllersWithViews();
			services.AddRazorPages();
			services.AddHttpContextAccessor();

			AddFirebaseAuth(CurrentEnvironment.IsDevelopment());
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
		{
			var isDevelopment = env.IsDevelopment();

			app.UseExceptionHandler(errorApp =>
			{
				errorApp.Run(async context =>  await ExceptionHandler.HandleGlobalExceptionAsync(context, isDevelopment));
			});

			app.UseRouting();

			if (isDevelopment)
			{
				app.UseCors("DevCorsPolicy");
				UseSwagger(app, provider);
			}
			else
			{
				app.UseCors();
				app.UseHttpsRedirection();
				app.UseHsts();
			}

			app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
				endpoints.MapHealthChecks("/health");

				if (isDevelopment)
				{
					endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
				}
            });
        }

		private void UseSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
		{
			app.UseSwagger();
			app.UseSwaggerUI(
				options =>
				{
					foreach (var description in provider.ApiVersionDescriptions)
					{
						var alias = string.IsNullOrWhiteSpace(Configuration["Swagger:Alias"])
							? ""
							: $"/{Configuration["Swagger:Alias"]}";

						options.SwaggerEndpoint($"{alias}/swagger/{description.GroupName}/swagger.json",
							description.GroupName.ToUpperInvariant());
					}
				});
		}

		private static void AddFirebaseAuth(bool isDevelopment)
		{
			var firebaseConfigFilePath =
				$"./brimschedule-firebase-admin-sdk{(isDevelopment ? "-test" : string.Empty)}.json";

			FirebaseApp.Create(new AppOptions
			{
				Credential = GoogleCredential.FromFile(firebaseConfigFilePath)
			});
		}
	}
}
