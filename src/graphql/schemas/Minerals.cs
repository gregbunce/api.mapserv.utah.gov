﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Minerals
    {
        public int Xid { get; set; }
        public string Zid { get; set; }
        public string RecNo { get; set; }
        public string RecType { get; set; }
        public string DepNum { get; set; }
        public string RepDate { get; set; }
        public string InfoSrc { get; set; }
        public string FilLink { get; set; }
        public string Rep { get; set; }
        public string RepAff { get; set; }
        public string Syn { get; set; }
        public string Dist { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phys { get; set; }
        public string Drain { get; set; }
        public string LandSt { get; set; }
        public string Quad1 { get; set; }
        public string Q1Scale { get; set; }
        public string Quad2 { get; set; }
        public decimal? Q2Scale { get; set; }
        public string Elev { get; set; }
        public decimal? UtmN { get; set; }
        public decimal? UtmE { get; set; }
        public decimal? UtmZ { get; set; }
        public string Acc { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public string Section { get; set; }
        public string SectFract { get; set; }
        public string Meridian { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string SiteName { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Commod { get; set; }
        public string OreMat { get; set; }
        public string ComSub { get; set; }
        public string GenAnal { get; set; }
        public string ComInfo { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public string Poten { get; set; }
        public string Occur { get; set; }
        public string NpMain { get; set; }
        public string NpMinor { get; set; }
        public string Prod { get; set; }
        public string LocalStru { get; set; }
        public decimal? Status { get; set; }
        public string Disc { get; set; }
        public string YrDisc { get; set; }
        public string NatDisc { get; set; }
        public string Yr1stPro { get; set; }
        public string YrLastPr { get; set; }
        public string Owner { get; set; }
        public string Oper { get; set; }
        public string ExplCom { get; set; }
        public string DepType { get; set; }
        public string DepForm { get; set; }
        public string DepthTop { get; set; }
        public string DepTU { get; set; }
        public string DepthBot { get; set; }
        public string DepBU { get; set; }
        public string MaxLen { get; set; }
        public string MLU { get; set; }
        public string MaxWid { get; set; }
        public string MWU { get; set; }
        public string MaxThick { get; set; }
        public string MTU { get; set; }
        public string DepSize { get; set; }
        public string Strike { get; set; }
        public string Dip { get; set; }
        public string PlungeDir { get; set; }
        public string Plunge { get; set; }
        public string DepDescC { get; set; }
        public string DepthWk { get; set; }
        public string DWkU { get; set; }
        public string LenWk { get; set; }
        public string LWkU { get; set; }
        public string OvLenWk { get; set; }
        public string OLU { get; set; }
        public string OvWidWk { get; set; }
        public string OWU { get; set; }
        public string OvAreaWk { get; set; }
        public string OAU { get; set; }
        public string DescWork { get; set; }
        public string HrAge { get; set; }
        public string HrType { get; set; }
        public string IgAge { get; set; }
        public string IgType { get; set; }
        public string MinAge { get; set; }
        public string NonOreMi { get; set; }
        public string OreCntl { get; set; }
        public string TectSet { get; set; }
        public string RegStruct { get; set; }
        public string Alter { get; set; }
        public string Conc { get; set; }
        public string FormAge { get; set; }
        public string FormName { get; set; }
        public string Form2Age { get; set; }
        public string Form2Name { get; set; }
        public string IgUnitAg { get; set; }
        public string IgUnitNa { get; set; }
        public string Ig2UnitA { get; set; }
        public string Ig2UnitN { get; set; }
        public string GeolCom { get; set; }
        public string GenCom { get; set; }
        public string Rf1 { get; set; }
        public string Rf2 { get; set; }
        public string Rf3 { get; set; }
        public string Rf4 { get; set; }
        public string DescWork1 { get; set; }
        public string ApItem { get; set; }
        public string ApAcc { get; set; }
        public string ApAmt { get; set; }
        public string ApU { get; set; }
        public string ApYear { get; set; }
        public string ApGrade { get; set; }
        public string CpItem { get; set; }
        public string CpAcc { get; set; }
        public string CpAmt { get; set; }
        public string CpU { get; set; }
        public string CpYear { get; set; }
        public string CpGrade { get; set; }
        public string PSource { get; set; }
        public string PCom { get; set; }
        public string RprItem { get; set; }
        public string RprAcc { get; set; }
        public string RprAmt { get; set; }
        public string RprU { get; set; }
        public string RprYear { get; set; }
        public string RprGrade { get; set; }
        public string RprSource { get; set; }
        public string RprCom { get; set; }
        public string RItem { get; set; }
        public string RAcc { get; set; }
        public string RAmt { get; set; }
        public string RU { get; set; }
        public string RYear { get; set; }
        public string RGrade { get; set; }
        public string RSource { get; set; }
        public string RCom { get; set; }
        public string PrItem { get; set; }
        public string PrAcc { get; set; }
        public string PrAmt { get; set; }
        public string PrU { get; set; }
        public string PrYear { get; set; }
        public string PrGrade { get; set; }
        public string PrSource { get; set; }
        public string PrCom { get; set; }
        public decimal? Ref { get; set; }
        public Point Shape { get; set; }
    }
}
