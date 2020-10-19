using AGRC.api.Features.Geocoding;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Extensions {
    public static class ConversionExtensions {
        public static SingleGeocodeResponseContract ToResponseObject(this Candidate candidate, string street, string zone) =>
            new SingleGeocodeResponseContract {
                MatchAddress = candidate.Address,
                Score = candidate.Score,
                Locator = candidate.Locator,
                Location = candidate.Location,
                AddressGrid = candidate.AddressGrid,
                InputAddress = $"{street}, {zone}",
                ScoreDifference = candidate.ScoreDifference
            };

        public static ReverseGeocodeResponseContract ToResponseObject(this ReverseGeocodeRestResponse response,
                                                                 Point inputPoint) => new ReverseGeocodeResponseContract {
                                                                     InputPoint = inputPoint,
                                                                     Address = response.Address.Street,
                                                                     Grid = response.Address.City,
                                                                     MatchPoint = response.Location
                                                                 };
    }
}