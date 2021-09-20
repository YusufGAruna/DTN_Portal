using Aruna_DTN_Entegrasyon.Filters;
using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Controllers
{

    
    public class HomeController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();

        [Authorize]
        public ActionResult Index()
        {
            var model = MyBll.GetUsers(User.Identity.Name);
            var type = model.Rol;
            if (type == RolType.Admin)
            {
                TempData["Admin"] = true;
            }
            else
            {
                TempData["Admin"] = false;
            }
            return View();
        }
        
        
    }
}