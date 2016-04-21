﻿using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class ChooseBestAddressCandidateCommand : Command<GeocodeAddressResult>
    {
        public ChooseBestAddressCandidateCommand(IEnumerable<Candidate> candidates, GeocodeOptions geocodeOptions, string street, string zone, GeocodeAddress geocodedAddress)
        {
            GeocodeOptions = geocodeOptions;
            Street = street;
            Zone = zone;
            GeocodedAddress = geocodedAddress;

            if (candidates == null)
            {
                candidates = new List<Candidate>();
            }

            Candidates = candidates.ToList();
        }

        public GeocodeOptions GeocodeOptions { get; set; }
        public string Street { get; set; }
        public string Zone { get; set; }
        public GeocodeAddress GeocodedAddress { get; set; }
        public List<Candidate> Candidates { get; set; }

        protected override void Execute()
        {
            if (Candidates == null || !Candidates.Any())
            {
                Result = new GeocodeAddressResult
                    {
                        InputAddress = string.Format("{0}, {1}", Street, Zone),
                        Score = -1
                    };

                return;
            }

            //get best match from candidates
            var result = Candidates.FirstOrDefault(x => 
                x.Score >= GeocodeOptions.AcceptScore &&
                GeocodedAddress.AddressGrids.Select(y => y.Grid).Contains(x.AddressGrid)) ?? new Candidate();

            //remove the result from the candidate list if it meets the accept score since it is the match address
            if (GeocodeOptions.SuggestCount > 0 &&
                result.Score >= GeocodeOptions.AcceptScore)
            {
                Candidates.Remove(result);
            }

            if (GeocodeOptions.SuggestCount == 0)
            {
                Candidates.Clear();
            }

            if (result.Location == null && GeocodeOptions.SuggestCount == 0)
            {
                Result = null;
                return;
            }

            var model = new GeocodeAddressResult
            {
                MatchAddress = result.Address,
                Score = result.Score,
                Locator = result.Locator,
                Location = result.Location,
                AddressGrid = result.AddressGrid,
                InputAddress = string.Format("{0}, {1}", Street, Zone),
                Candidates = Candidates.Take(GeocodeOptions.SuggestCount).ToArray()
            };

            var standard = GeocodedAddress.StandardizedAddress.ToLowerInvariant();
            var input = Street.ToLowerInvariant();

            if (input != standard)
            {
                model.StandardizedAddress = standard;
            }
            
            Result = model;
        }

        public override string ToString()
        {
            return string.Format("{0}, GeocodeOptions: {1}, Candidates: {2}", "ChooseWinnerCommand2", GeocodeOptions,
                                 Candidates.Count);
        }
    }
}