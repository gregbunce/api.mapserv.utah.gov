using System;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace graphql {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            Configuration = configuration;
            Environment = env;
        }

        public IWebHostEnvironment Environment { get; set; }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<CloudSqlContext>(options => options.UseNpgsql(Configuration.GetConnectionString("OpenSGID"), o => o.UseNetTopologySuite()));

            services.AddHttpClient("geocoder", o => {
                o.Timeout = TimeSpan.FromSeconds(5);
            });

            services.AddGraphQL(sb => SchemaBuilder.New()
                .AddType<types.MultiPolygonType>()
                .AddType<types.MultiLineType>()
                .AddType<types.PointType>()
                .AddQueryType<Query>()
                .ModifyOptions(o => o.RemoveUnreachableTypes = true).Create(),
                new QueryExecutionOptions {
                    IncludeExceptionDetails = Environment.IsDevelopment()
                });
        }
        public void Configure(IApplicationBuilder app) => app.UseRouting().UseWebSockets().UseGraphQL().UsePlayground().UseVoyager();
    }
}