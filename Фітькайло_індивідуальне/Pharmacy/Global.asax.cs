using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Pharmacy.Models;

namespace Pharmacy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {   
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
    }
}
