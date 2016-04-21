﻿using System.Web.Optimization;

namespace WebAPI.API
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-1.10.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                                         "//netdna.bootstrapcdn.com/twitter-bootstrap/2.2.2/js/bootstrap.min.js").
                            Include(
                                "~/Scripts/bootstrap.js",
                                "~/Scripts/scrollspy.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/Scripts/modernizr-2.6.1.js",
                "~/Scripts/site.js"));

            bundles.Add(new StyleBundle("~/Content/bundles").Include(
                "~/Content/bootstrap.css",
                "~/Content/Site.css"));
        }
    }
}