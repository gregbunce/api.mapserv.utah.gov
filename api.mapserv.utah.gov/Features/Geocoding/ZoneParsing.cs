﻿using System;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using MediatR;
using Serilog;
using System.Threading.Tasks;

namespace api.mapserv.utah.gov.Features.Geocoding
{
    public class ZoneParsing
    {
        public class Command : IRequest<GeocodeAddress> {
            public string InputZone { get; set; }
            public GeocodeAddress AddressModel { get; set; }

            public Command(string inputZone, GeocodeAddress addressModel)
            {
                InputZone = inputZone;
                AddressModel = addressModel;
            }
        }

        public class Handler : IRequestHandler<Command, GeocodeAddress> {
            private readonly IRegexCache _regex;
            private readonly ILookupCache _driveCache;
            private readonly IMediator _mediator;

            public Handler(IRegexCache regex, ILookupCache driveCache, IMediator mediator)
            {
                _regex = regex;
                _driveCache = driveCache;
                _mediator = mediator;
            }

            public async Task<GeocodeAddress> Handle(Command request, CancellationToken token)
            {
                Log.Debug("Parsing {zone}", request.InputZone);

                request.InputZone = request.InputZone.Replace("-", "");
                var zipPlusFour = _regex.Get("zipPlusFour").Match(request.InputZone);

                if (zipPlusFour.Success)
                {

                    if (zipPlusFour.Groups[1].Success)
                    {
                        var zip5 = zipPlusFour.Groups[1].Value;
                        Log.Debug("Zone has a zip code of {zip}", zip5);

                        request.AddressModel.Zip5 = int.Parse(zip5);

                        var getAddressSystemFromZipCodeCommand = new AddressSystemFromZipCode.Command(request.AddressModel.Zip5);

                        request.AddressModel.AddressGrids = await _mediator.Send(getAddressSystemFromZipCodeCommand);
                    }

                    if (zipPlusFour.Groups[2].Success)
                    {
                        var zip4 = zipPlusFour.Groups[2].Value;

                        Log.Debug("Zone has a zip + 4 {zip}", zip4);

                        request.AddressModel.Zip4 = int.Parse(zip4);
                    }

                    var doubleAves = new DoubleAvenuesException.Command(request.AddressModel, "");

                    return await _mediator.Send(doubleAves);
                }

                var cityName = _regex.Get("cityName").Match(request.InputZone);

                if (cityName.Success)
                {
                    Log.Debug("Zone is a place {place}", cityName.Value);

                    var cityKey = cityName.Value.ToLower();
                    cityKey = cityKey.Replace(".", "");
                    cityKey = Regex.Replace(cityKey, @"\s+", " ");

                    cityKey = _regex.Get("cityTownCruft").Replace(cityKey, "").Trim();

                    var getAddressSystemFromCityCommand = new AddressSystemFromCity.Command(cityKey);
                    request.AddressModel.AddressGrids = await _mediator.Send(getAddressSystemFromCityCommand);

                    var doubleAves = new DoubleAvenuesException.Command(request.AddressModel, cityKey);

                    return await _mediator.Send(doubleAves);
                }

                if (request.AddressModel.AddressGrids == null)
                {
                    Log.Warning("No address grid found for {zone}", request.InputZone);

                    request.AddressModel.AddressGrids = Enumerable.Empty<GridLinkable>().ToList();
                }

                return request.AddressModel;
            }
        }
    }
}