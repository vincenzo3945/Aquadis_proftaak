using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace F1_Manager.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.MapHttpAttributeRoutes();

            configuration.Formatters.JsonFormatter.SupportedMediaTypes
            .Add(new MediaTypeHeaderValue("text/html"));

            
            configuration.Routes.MapHttpRoute("GroupTwoParameters", "api/{controller}/{action}/{username}/{groupname}",
              new { username = RouteParameter.Optional, groupname = RouteParameter.Optional });

            configuration.Routes.MapHttpRoute("GroupOneParameter", "api/{controller}/{action}/{username}",
              new { username = RouteParameter.Optional });

            /*configuration.Routes.MapHttpRoute("GetGroup", "api/{controller}/{action}/{parameter}",
              new {parameter = RouteParameter.Optional });*/

            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
              new { id = RouteParameter.Optional });
        }
    }
}