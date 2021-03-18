using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using BrimSchedule.API.Services;
using BrimSchedule.API.SwaggerConfiguration;
using BrimSchedule.Domain.Models;
using BrimSchedule.Persistence.EF;
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
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrimSchedule.API
{
	public class Startup
	{
		private const string Bearer = "Bearer";
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
					OnMessageReceived = async context =>
					{
						var authorization = context.Request.Headers["Authorization"].ToString();

						if (string.IsNullOrEmpty(authorization))
						{
							context.NoResult();
							return;
						}

						if (!authorization.StartsWith(Bearer, StringComparison.OrdinalIgnoreCase)) return;

						var token = authorization.Substring(Bearer.Length).Trim();

						if (string.IsNullOrEmpty(token))
						{
							context.NoResult();
							return;
						}

						var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).ConfigureAwait(false);

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

					options.AddSecurityDefinition(Bearer, new OpenApiSecurityScheme()
					{
						In = ParameterLocation.Header,
						Description = $"Введите в поле JWT токен с припиской {Bearer}",
						Name = "Authorization",
						Type = SecuritySchemeType.ApiKey
					});

					options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme()
							{
								Name = Bearer
							},
							new List<string>()
						}
					});
				});

			services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<BrimScheduleContext>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(errorApp =>
				{
					errorApp.Run(async context => { await ExceptionHandler.HandleGlobalExceptionAsync(context); });
				});
			}

			app.UseRouting();

			if (env.IsDevelopment())
			{
				app.UseCors("DevCorsPolicy");

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
			}
			else
			{
				app.UseCors();
				app.UseHttpsRedirection();
				app.UseHsts();
			}

			var firebaseConfigFilePath =
				$"./brimschedule-firebase-admin-sdk{(env.IsDevelopment() ? "-test" : string.Empty)}.json";

			FirebaseApp.Create(new AppOptions()
			{
				Credential = GoogleCredential.FromFile(firebaseConfigFilePath)
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
