using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Filters
{
    public class CustomAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {


            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {

                if (!HttpContext.Current.Response.IsRequestBeingRedirected)
                    filterContext.HttpContext.Response.Redirect("~/Login/Login");


            }


        }
    }
}