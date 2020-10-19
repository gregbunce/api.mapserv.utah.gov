﻿namespace AGRC.api.Features.Geocoding {
    public class LocatorMetadata {
        public LocatorMetadata(AddressWithGrids addressInfo, string grid, int weight, LocatorProperties locator,
                            int wkId = 26912) {
            var street = addressInfo.StandardizedAddress;

            AddressInfo = addressInfo;
            Address = street;
            Grid = grid;
            Weight = weight;
            Locator = locator;
            WkId = wkId;
        }

        public AddressWithGrids AddressInfo { get; set; }
        public string Address { get; set; }
        public string Grid { get; set; }
        public int Weight { get; set; }
        public LocatorProperties Locator { get; set; }
        public int WkId { get; set; }
    }
}