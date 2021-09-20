using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    public class AdminController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult KullaniciTanim()
        {
            var users = MyBll.GetAllUsers();
            return View(users);
        }

        public ActionResult GrupKoduTanim()
        {
            var kodlar = MyBll.GetTanimliGrupKodlari();
            return View(kodlar);
        }

        public ActionResult RaporTanim()
        {
            var raporlar = MyBll.GetTanimliRaporlar();
            return View(raporlar);
        }

        [Authorize]
        public ActionResult GrupTanimCreate()
        {
            var items = MyBll.GetGrupKodlari();
            List<SelectListItem> grupList = new List<SelectListItem>();
            foreach (DataRow dr in items.Rows)
            {
                SelectListItem selectitem = new SelectListItem();
                selectitem.Text = dr["GrupAdi"].ToString();
                selectitem.Value = dr["GrupAdi"].ToString();
                grupList.Add(selectitem);
            }

            TempData["Gruplar"] = grupList;
            return View();

        }

        [Authorize]
        public ActionResult RaporTanimCreate()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult GrupTanimEdit(string GrupAdi, bool IsChecked)
        {
            try
            {
                MyBll.UpdateGrup(GrupAdi, IsChecked);
                return RedirectToAction("GrupKoduTanim");
            }
            catch (Exception exc)
            {
                
                ModelState.AddModelError("", exc.Message + Environment.NewLine + exc.InnerException.Message != null ? exc.InnerException.Message : string.Empty);
                return View();
            }

        }


        [HttpPost]
        [Authorize]
        public ActionResult RaporTanimEdit(string RaporAdi, string ViewAdi)
        {
            try
            {
                MyBll.UpdateRapor(RaporAdi, ViewAdi);
                return RedirectToAction("RaporTanim");
            }
            catch (Exception exc)
            {

                ModelState.AddModelError("", exc.Message + Environment.NewLine + exc.InnerException.Message != null ? exc.InnerException.Message : string.Empty);
                return View();
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult GrupTanimCreate(string GrupAdi,bool IsChecked)
        {
            try
            {
                MyBll.InsertGrup(GrupAdi, IsChecked);
                return RedirectToAction("GrupKoduTanim");
            }
            catch(Exception exc)
            {
                var grupList = (List<SelectListItem>)TempData["Gruplar"];
                TempData["Gruplar"] = grupList;
                ModelState.AddModelError("", exc.Message + Environment.NewLine + exc.InnerException.Message != null ? exc.InnerException.Message : string.Empty);
                return View();
            }
            

        }

        [HttpPost]
        [Authorize]
        public ActionResult RaporTanimCreate(string RaporAdi, string ViewAdi)
        {
            try
            {
                MyBll.InsertRapor(RaporAdi, ViewAdi);
                return RedirectToAction("RaporTanim");
            }
            catch (Exception exc)
            {                
                ModelState.AddModelError("", exc.Message + Environment.NewLine + exc.InnerException.Message != null ? exc.InnerException.Message : string.Empty);
                return View();
            }


        }


        public ActionResult GrupTanimDelete(string Grup)
        {
            MyBll.DeleteGrup(Grup);
            var users = MyBll.GetTanimliGrupKodlari();
            return RedirectToAction("GrupKoduTanim", "Admin");
        }

        public ActionResult RaporTanimDelete(string Rapor)
        {
            MyBll.DeleteRapor(Rapor);
            var users = MyBll.GetTanimliRaporlar();
            return RedirectToAction("RaporTanim", "Admin");
        }

        [Authorize]
        public ActionResult Create()
        {
            TempData["Plasiyerler"] = MyBll.GetPlasiyer();
            return View();
        }

        [Authorize]
        public ActionResult GrupTanimEdit(string Grup)
        {
            TempData["Grup"] = Grup;
            return View();
        }

        [Authorize]
        public ActionResult RaporTanimEdit(string Rapor)
        {
            TempData["Rapor"] = Rapor;
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(Kullanicilar kullanici)
        {
            try
            {
                 MyBll.InsertKullanici(kullanici);

                

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", new Exception("Insert sırasında hata oluştu"));
                return View(kullanici);
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(string id)
        {
            var users = MyBll.GetAllUsers().Where(t => t.UserName == id).First();

            return View(users);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(Kullanicilar kullanici)
        {
            try
            {
                MyBll.DeleteKullanici(kullanici.UserName);
                MyBll.InsertKullanici(kullanici);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
        {
            MyBll.DeleteKullanici(id);
            var users = MyBll.GetAllUsers();
            return RedirectToAction("Index","Admin");
        }
        
    }
}
