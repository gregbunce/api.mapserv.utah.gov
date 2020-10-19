using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AGRC.api.Models.ArcGis {
    public class PointReprojectOptions {
        public PointReprojectOptions(int currentSpatialReference, int reprojectToSpatialReference,
                                     IReadOnlyCollection<double> coordinates) {
            CurrentSpatialReference = currentSpatialReference;
            ReprojectToSpatialReference = reprojectToSpatialReference;
            Coordinates = coordinates;
        }

        [JsonPropertyName("inSR")]
        public int CurrentSpatialReference { get; private set; }

        [JsonPropertyName("outSR")]
        public int ReprojectToSpatialReference { get; private set; }

        [JsonPropertyName("geometries")]
        public IReadOnlyCollection<double> Coordinates { get; private set; }
    }
}