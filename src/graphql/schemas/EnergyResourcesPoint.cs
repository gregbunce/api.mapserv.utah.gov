﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnergyResourcesPoint
    {
        public int Xid { get; set; }
        public string Code { get; set; }
        public Point Shape { get; set; }
    }
}
