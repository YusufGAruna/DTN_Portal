using Aruna_DTN_Entegrasyon.Models;
using Aruna_DTN_Entegrasyon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    
    public class LoginController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                

                var personel = MyBll.GetLogin(model.UserName, model.Password);

                if (personel == null)
                {
                    ModelState.AddModelError("", "Kullanıcı Adı veya şifre hatalı!");
                    return View(model);
                }
                
                
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); 
            return RedirectToAction("Login", "Login");
        }
    }
}