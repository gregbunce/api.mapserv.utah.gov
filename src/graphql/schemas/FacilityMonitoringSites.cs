﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class FacilityMonitoringSites
    {
        public int Xid { get; set; }
        public string Storet { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Hydrologic { get; set; }
        public string County { get; set; }
        public decimal? Storets { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public string Unit { get; set; }
        public decimal? Rec { get; set; }
        public string LatDms83 { get; set; }
        public string LongDms83 { get; set; }
        public Point Shape { get; set; }
    }
}
