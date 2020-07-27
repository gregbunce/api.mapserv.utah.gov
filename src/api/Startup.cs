using System;
using System.IO;
using System.Reflection;
using AGRC.api.Extensions;
using AGRC.api.Features.Health;
using AGRC.api.Infrastructure;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;
using CorrelationId.DependencyInjection;
using CorrelationId;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using AGRC.api.Features.Geocoding;
using AGRC.api.Models;
using AGRC.api.Features.GeometryService;

namespace AGRC.api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDefaultCorrelationId(options => {
                options.AddToLoggingScope = true;
                options.EnforceHeader = false;
                options.IgnoreRequestHeader = true;
                options.IncludeInResponse = false;
                options.UpdateTraceIdentifier = false;
            });

            services.AddCors(options => {
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            services.AddControllers(options => {
                options.AddApiResponseFormatters();
                // options.AddJsonpOutputFormatter();
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddApiVersioning(x => {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.Conventions.Add(new VersionByNamespaceConvention());
            });

            services.AddHealthChecks()
                    .AddCheck<CacheHealthCheck>("Cache", failureStatus: HealthStatus.Degraded)
                    .AddCheck<KeyStoreHealthCheck>("KeyStore", failureStatus: HealthStatus.Unhealthy)
                    .AddCheck<GeometryServiceHealthCheck>("GeometryService", failureStatus: HealthStatus.Degraded)
                    .AddCheck<LocatorHealthCheck>("Geolocators");

            services.UseOptions(Configuration);
            services.UseDi();

            services.AddSwaggerGen(c => {
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                // c.DescribeAllEnumsAsStrings();
                // c.DescribeStringEnumsInCamelCase();
                c.CustomOperationIds(apiDesc => {
                    return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
                });

                c.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "AGRC WebAPI : OpenAPI Documentation",
                    Description = "OpenAPI Documentation",
                    Contact = new OpenApiContact {
                        Name = "AGRC",
                        Email = "sgourley@utah.gov",
                        Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov")
                    },
                    License = new OpenApiLicense {
                        Name = "MIT",
                        Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov/blob/master/LICENSE")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            // set up computations
            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(typeof(IComputationHandler<,>))
                    .AsImplementedInterfaces();

            builder.RegisterDecorator<DoubleAvenuesException.Decorator,
                                      IComputationHandler<ZoneParsing.Computation, AddressWithGrids>>();


            builder.RegisterGenericDecorator(typeof(Reproject.Decorator<,>), typeof(IComputationHandler<,>));

            builder.Register(c => new ComputeMediator(c.Resolve<IComponentContext>().Resolve))
                   .AsImplementedInterfaces()
                   .SingleInstance();

            // set up mediatr
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler <,>))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx => {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseCorrelationId();

            app.UseRouting();

            app.UseCors();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c => {
                c.SerializeAsV2 = true;
                c.RouteTemplate = "openapi/{documentName}/api.json";
            });

            app.UseSwaggerUI(c => {
                c.DocumentTitle = "AGRC WebAPI OpenAPI Documentation";
                c.RoutePrefix = "openapi";
                c.SwaggerEndpoint("/openapi/v1/api.json", "v1");
                c.SupportedSubmitMethods();
                c.EnableDeepLinking();
                c.DocExpansion(DocExpansion.List);
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/v1/health/details", new HealthCheckOptions {
                    ResponseWriter = HealthCheckResponseWriter.WriteDetailsJson
                });
                endpoints.MapHealthChecks("/api/v1/health");
                endpoints.MapHealthChecks("", new HealthCheckOptions() {
                    Predicate = (check) => false
                });
            });
        }
    }
}
