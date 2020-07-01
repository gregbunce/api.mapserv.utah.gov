using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.mapserv.utah.gov.Filters {
    public class JsonOutputFormatResultFilter : IAsyncResultFilter {
        private readonly IComputeMediator _mediator;

        public JsonOutputFormatResultFilter(IComputeMediator mediator) {
            _mediator = mediator;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var format = context.HttpContext.Request.Query["format"];
            if (string.IsNullOrEmpty(format)) {
                await next();

                return;
            }

            var response = context.Result as ObjectResult;

            if (response.Value is ApiResponseContainer<GeocodeAddressApiResponse> container) {
                switch (format.ToString().ToLowerInvariant()) {
                    case "geojson": {
                        var command = new GeoJsonFeature.Computation(container);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                    case "esrijson": {
                        var command = new EsriGraphic.Computation(container);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                    default: {
                        break;
                    }
                }
            }

            await next();
        }
    }
}
