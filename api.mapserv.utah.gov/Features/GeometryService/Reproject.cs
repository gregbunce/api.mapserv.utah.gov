﻿using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Formatters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.SecretOptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace api.mapserv.utah.gov.Features.GeometryService
{
    public class Reproject
    {
        public class Command : IRequest<ReprojectResponse<Point>>
        {
            internal readonly PointReprojectOptions Options;

            public string ReprojectUrl { get; set; }

            public Command(PointReprojectOptions options)
            {
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Command, ReprojectResponse<Point>>
        {
            private readonly IOptions<GeometryServiceConfiguration> _geometryServiceConfiguration;
            private readonly HttpClient _client;
            private readonly MediaTypeFormatter[] _mediaTypes;

            public Handler(IOptions<GeometryServiceConfiguration> geometryServiceConfiguration,
                                      IHttpClientFactory clientFactory)
            {
                _geometryServiceConfiguration = geometryServiceConfiguration;
                _client = clientFactory.CreateClient("default");
                _mediaTypes = new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                };
            }

            public async Task<ReprojectResponse<Point>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.ReprojectUrl))
                {
                    request.ReprojectUrl = _geometryServiceConfiguration.Value.ToString();
                }

                var query = new QueryString("?f=json");
                query = query.Add("outSR", request.Options.ReprojectToSpatialReference.ToString());
                query = query.Add("inSR", request.Options.CurrentSpatialReference.ToString());
                query = query.Add("geometries", string.Join(",", request.Options.Coordinates));

                var requestUri = string.Format(request.ReprojectUrl, query.Value);

                Log.Debug("Repojecting {args} with {url}", request.Options, request.ReprojectUrl);

                var response = await _client.GetAsync(requestUri);
                var result = await response.Content.ReadAsAsync<ReprojectResponse<Point>>(_mediaTypes);

                return result;
            }
        }
    }
}
