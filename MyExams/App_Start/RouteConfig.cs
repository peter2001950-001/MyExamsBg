using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyExams
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.MapRoute(
                name: "TeacherIndex",
                url: "t/main",
                defaults: new { controller = "Teacher", action = "Index" }
                );
            routes.MapRoute(
                name: "Teacher",
                url: "t/{action}",
                defaults: new { controller = "Teacher", action = "Index" }
                );
            routes.MapRoute(
                name: "Student",
                url: "s/{action}",
                defaults: new { controller = "Student", action = "Index" }
                );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
