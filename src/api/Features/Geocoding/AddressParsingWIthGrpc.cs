using System;
using System.Threading;
using System.Threading.Tasks;
using Addressparser;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class AddressParsingWithGrpc {
        public class Command : IRequest<CleansedAddress> {
            public Command(string street) {
                Street = street;
            }

            public string Street { get; set; }
        }

        public class Handler : IRequestHandler<Command, CleansedAddress> {
            private readonly ILogger _log;
            private readonly AddressService.AddressServiceClient _client;

            public Handler(ILogger log, AddressService.AddressServiceClient client) {
                _log = log;
                _client = client;
            }

            public async Task<CleansedAddress> Handle(Command request, CancellationToken cancellationToken) {
                _log.Debug("Parsing {street}", request.Street);

                var result = await _client.ParseAsync(new InputAddress { Address = request.Street });

                var prefixDirection = Direction.None;
                if (!string.IsNullOrEmpty(result.PrefixDirection)) {
                    switch (result.PrefixDirection)
                    {
                        case "N":{
                            prefixDirection = Direction.North;
                            break;
                        }
                        case "E":{
                            prefixDirection = Direction.East;
                            break;
                        }
                        case "S":{
                            prefixDirection = Direction.South;
                            break;
                        }
                        case "W":{
                            prefixDirection = Direction.West;
                            break;
                        }
                        default:
                            prefixDirection = Direction.None;
                            break;
                    }
                }
                Enum.TryParse<Direction>(result.AddressNumberSuffix, out var suffixDirection);
                Enum.TryParse<StreetType>(result.StreetType, out var streetType);

                var isPoBox = false;
                var pobox = -1;
                if (!string.IsNullOrEmpty(result.PoBox)) {
                    pobox = Convert.ToInt32(result.PoBox);
                    isPoBox = true;
                }

                _log.Information("Replaced {original} with {standard}", request.Street, result.Normalized);

                return new CleansedAddress(request.Street,
                                           Convert.ToInt32(result.AddressNumber),
                                           0,
                                           pobox,
                                           prefixDirection,
                                           result.StreetName,
                                           streetType,
                                           suffixDirection,
                                           0,
                                           null,
                                           false,
                                           isPoBox);
            }
        }
    }
}
