﻿using System.Web;
using System.Web.Optimization;

namespace MyExams
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                       "~/Scripts/knockout-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/classesViewModel").Include(
                       "~/Scripts/app/classesViewModel.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jquery.unobtrusive-ajax").Include(
                       "~/Scripts/jquery.unobtrusive-ajax.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                      "~/Scripts/dashboard.js", "~/Scripts/admin.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/dashboard").Include(
                "~/Content/material-icons.css",
                     "~/Content/Admin.css",
                     "~/Content/jquery-ui.css",
                     "~/Content/Site.css"
                     ));
            bundles.Add(new StyleBundle("~/Content/main").Include(
                     "~/Content/site.css"));

        }
    }
}
