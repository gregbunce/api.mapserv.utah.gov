﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Web;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class GetLocatorsForAddressCommand : Command<ReadOnlyCollection<LocatorDetails>>
    {
        public GetLocatorsForAddressCommand(GeocodeAddress address, GeocodeOptions options)
        {
            Host = ConfigurationManager.AppSettings["gis_server_host"];
            Address = address;
            Options = options;

            BuildAddressPermutations();
            BuildLocatorLookup();
        }

        public GeocodeAddress Address { get; set; }

        public string Host { get; set; }

        public GeocodeOptions Options { get; set; }

        private IDictionary<LocatorType, List<LocatorDetails>> LocatorLookup { get; set; }

        protected List<GeocodeOperationInput> AddressPermutations { get; set; }

        private void BuildAddressPermutations()
        {
            AddressPermutations = new List<GeocodeOperationInput>();

            if (!Address.AddressGrids.Any())
            {
                return;
            }

            foreach (var grid in Address.AddressGrids)
            {
                AddressPermutations.Add(new GeocodeOperationInput(Address, grid.Grid, grid.Weight, null, Options.WkId));
            }
        }

        private void BuildLocatorLookup()
        {
            var locatorLookup = new Dictionary<LocatorType, List<LocatorDetails>>();

            Add(UspsDeliveryPoint(), ref locatorLookup, LocatorType.All);
            Add(Intersection(), ref locatorLookup, LocatorType.All);
            Add(AddressPoints(), ref locatorLookup, LocatorType.AddressPoints);
            Add(Reversal(Address), ref locatorLookup, LocatorType.RoadCenterlines);
            Add(Centerlines(Address), ref locatorLookup, LocatorType.RoadCenterlines);

            LocatorLookup = locatorLookup;
        }

        protected override void Execute()
        {
            if (!Address.IsMachable())
            {
                return;
            }

            switch (Options.Locators)
            {
                case LocatorType.All:
                    Result = LocatorLookup.Values.SelectMany(x => x).ToList().AsReadOnly();
                    break;
                case LocatorType.AddressPoints:
                    if (!LocatorLookup.ContainsKey(Options.Locators))
                    {
                        break;
                    }

                    Result = LocatorLookup[LocatorType.AddressPoints].ToList().AsReadOnly();
                    break;
                case LocatorType.RoadCenterlines:
                    if (!LocatorLookup.ContainsKey(Options.Locators))
                    {
                        break;
                    }

                    Result = LocatorLookup[LocatorType.RoadCenterlines].ToList().AsReadOnly();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Options.Locators");
            }
        }

        private IEnumerable<LocatorDetails> Centerlines(AddressBase address)
        {
           
            var locators = new List<LocatorDetails>();
    
            if (address.IsReversal())
            {
                AddressPermutations.ForEach(stuff => locators.AddRange(new[]
                    {
                        new LocatorDetails
                            {
                                Url =
                                    string.Format("http://{0}", Host) +
                                    string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                                                  "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                                  HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                                Name = "Centerlines.StatewideRoads",
                                Weight = stuff.Weight
                            },
                        new LocatorDetails
                            {
                                Url =
                                    string.Format("http://{0}", Host) +
                                    string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                                                  "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                                  HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                                Name = "Centerlines.AddressCoordinateAlias",
                                Weight = stuff.Weight
                            }
                    }));

                return locators;
            }

            AddressPermutations.ForEach(stuff => locators.AddRange(new[]
                {
                    new LocatorDetails
                        {
                            Url =
                                string.Format("http://{0}", Host) +
                                string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                                              "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                              HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                            Name = "Centerlines.StatewideRoads",
                            Weight = stuff.Weight
                        },
                    new LocatorDetails
                        {
                            Url =
                                string.Format("http://{0}", Host) +
                                string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                                              "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                              HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                            Name = "Centerlines.AddressCoordinateAlias",
                            Weight = stuff.Weight
                        },
                    new LocatorDetails
                        {
                            Url =
                                string.Format("http://{0}", Host) +
                                string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_ALIAS1/" +
                                              "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                              HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                            Name = "Centerlines.PrimaryRoadAlias",
                            Weight = stuff.Weight
                        },
                    new LocatorDetails
                        {
                            Url =
                                string.Format("http://{0}", Host) +
                                string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_ALIAS2/" +
                                              "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                              HttpUtility.UrlEncode(stuff.Address), stuff.Grid, stuff.WkId),
                            Name = "Centerlines.SecondaryRoadAlias",
                            Weight = stuff.Weight
                        }
                }));

            return locators;
        }

        private IEnumerable<LocatorDetails> AddressPoints()
        {
            var locators = new List<LocatorDetails>();

            AddressPermutations.ForEach(x =>
                {
                    if (!App.GridsWithAddressPoints.Contains(x.Grid)) 
                        return;

                    if (x.AddressInfo.IsReversal() || x.AddressInfo.PossibleReversal())
                    {
                        locators.Add(new LocatorDetails
                            {
                                Url = string.Format("http://{0}", Host) +
                                      "/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                                      string.Format(
                                          "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                          HttpUtility.UrlEncode(x.AddressInfo.ReversalAddress), x.Grid, x.WkId),
                                Name = "AddressPoints.AddressGrid",
                                Weight = x.Weight
                            });
                    }

                    locators.Add(new LocatorDetails
                        {
                            Url = string.Format("http://{0}", Host) +
                                  "/arcgis/rest/services/Geolocators/AddressPoints_AddressSystem/" +
                                  string.Format(
                                      "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                      HttpUtility.UrlEncode(x.Address), x.Grid, x.WkId),
                            Name = "AddressPoints.AddressGrid",
                            Weight = x.Weight
                        });
                });

            return locators;
        }

        /// <summary>
        ///     Determines if the address is likely a reversal
        ///     case 1: (300 S 437 E) where the street name ends in 2,3,4,6,7,8,9
        ///     case 2: (350 S 435 E) where street name and house number both end in a 0 or 5
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>locator_SwitchRoadandHouseNumber</returns>
        private IEnumerable<LocatorDetails> Reversal(AddressBase address)
        {
            if (address.IsReversal() || address.PossibleReversal())
            {
                var locators = new List<LocatorDetails>();
                
                AddressPermutations.ForEach(stuff => locators.AddRange(new[]
                    {
                        new LocatorDetails
                            {
                                Url = string.Format("http://{0}", Host) +
                                      string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_STREET/" +
                                                    "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                                    HttpUtility.UrlEncode(stuff.AddressInfo.ReversalAddress), stuff.Grid, stuff.WkId),
                                Name = "Centerlines.StatewideRoads"
                            },
                        new LocatorDetails
                            {
                                Url = string.Format("http://{0}", Host) +
                                      string.Format("/arcgis/rest/services/Geolocators/Roads_AddressSystem_ACSALIAS/" +
                                                    "GeocodeServer/findAddressCandidates?f=json&Street={0}&City={1}&outSR={2}",
                                                    HttpUtility.UrlEncode(stuff.AddressInfo.ReversalAddress), stuff.Grid, stuff.WkId),
                                Name = "Centerlines.AddressCoordinateAlias"
                            }
                    }));

                return locators;
            }

            return Enumerable.Empty<LocatorDetails>();
        }

        /// <summary>
        ///     Determines if the address is an intersection.
        ///     If the word and is found in the address this is true
        /// </summary>
        /// <returns>locator_IntersectionPoints</returns>
        private static IEnumerable<LocatorDetails> Intersection()
        {
            return Enumerable.Empty<LocatorDetails>();
        }

        /// <summary>
        ///     If the address is a usps delivery point the usps locator will be added
        /// </summary>
        /// <returns>locator_DeliveryPoints</returns>
        private static IEnumerable<LocatorDetails> UspsDeliveryPoint()
        {
            return Enumerable.Empty<LocatorDetails>();
        }

        /// <summary>
        ///     Adds the specified locator to the locators list if the return value is not null.
        /// </summary>
        /// <param name="locatorsToAdd">The locator.</param>
        /// <param name="lookup">The lookup.</param>
        /// <param name="key">The key.</param>
        private static void Add(IEnumerable<LocatorDetails> locatorsToAdd,
                                ref Dictionary<LocatorType, List<LocatorDetails>> lookup, LocatorType key)
        {
            if (locatorsToAdd == null)
            {
                return;
            }

            locatorsToAdd = locatorsToAdd.ToList();

            if (!locatorsToAdd.Any())
            {
                return;
            }

            if (!lookup.ContainsKey(key))
            {
                lookup.Add(key, locatorsToAdd.ToList());
            }
            else
            {
                lookup[key].AddRange(locatorsToAdd);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Address: {1}", "GetLocatorsForAddress", Address);
        }
    }
}