﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UsCongressDistricts2002
    {
        public int Xid { get; set; }
        public decimal? District { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}