using Aruna_DTN_Entegrasyon.Models;
using Aruna_DTN_Entegrasyon.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    public class SaticiSiparisController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();

        [Authorize]
        public ActionResult Index()
        {
            List<Siparisler> siparisler = MyBll.GetSaticiSiparisler();
            return View(siparisler);
        }
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Siparisler siparisler = MyBll.GetSaticiSiparisler().Where(t => t.FatNo == id).First();
            if (siparisler == null)

            {
                return HttpNotFound();
            }
            TempData["FatNo"] = siparisler.FatNo;
            List<SaticiSiparisKalemler> faturaKalemler = MyBll.GetSaticiSiparisKalemler(siparisler.FatNo);
            return View(model: id);
        }
        [Authorize]
        public ActionResult Create()
        {
            Siparisler siparis = new Siparisler();
            var userID = User.Identity.Name;

            Kullanicilar kullanici = MyBll.GetUsers(userID);
            siparis.FatNo = MyBll.GetFatNo("SatSiparis", kullanici.UserName);
            siparis.Plasiyer = kullanici.UserName;
            siparis.Tarih = DateTime.Today;
            siparis.OdemeTarih = DateTime.Today;
            siparis.Cari = new Cariler();
            TempData["Stoklar"] = MyBll.GetStoklar(true);
            if (TempData["Kalemler"] != null)
            {
             
                var kalems = TempData["Kalemler"];
                TempData["Kalemler"] = kalems;
            }
            else
            {
                TempData["Kalemler"] = new List<KalemViewModel>();
            }
            TempData["Depolar"] = MyBll.GetDepolar();
            ViewBag.Kullanici = kullanici;
            return View(siparis);
        }
        [HttpPost]
        public ActionResult Create(Siparisler siparis)
        {

            var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];
            try
            {
                siparis.FatNo = siparis.FatNo.PadLeft(15, '0');

                if (string.IsNullOrEmpty(siparis.Cari.CariKod) || siparis.Cari.CariKod == Parametreler.MuhtelifCari)
                {
                    var userID = User.Identity.Name;
                    Kullanicilar kullanici = MyBll.GetUsers(userID);
                    TempData["Stoklar"] = MyBll.GetStoklar(true);
                    TempData["Kalemler"] = kalemModel;
                    TempData["Depolar"] = MyBll.GetDepolar();
                    ViewBag.Kullanici = kullanici;
                    ModelState.AddModelError("", "Lütfen Cari Giriniz");
                    return View(siparis);
                }
                var result = MyBll.SaticiSiparis(siparis, kalemModel);

                if (!result.IsSuccessful)
                {
                    var userID = User.Identity.Name;
                    Kullanicilar kullanici = MyBll.GetUsers(userID);
                    TempData["Stoklar"] = MyBll.GetStoklar(true);
                    TempData["Kalemler"] = kalemModel;
                    TempData["Depolar"] = MyBll.GetDepolar();
                    ViewBag.Kullanici = kullanici;
                    ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + result.ErrorDesc);
                    return View(siparis);
                }
                TempData["Kalemler"] = new List<KalemViewModel>();
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                var userID = User.Identity.Name;
                Kullanicilar kullanici = MyBll.GetUsers(userID);
                TempData["Stoklar"] = MyBll.GetStoklar(true);
                TempData["Kalemler"] = kalemModel;
                TempData["Depolar"] = MyBll.GetDepolar();
                ViewBag.Kullanici = kullanici;
                ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + exc.Message);
                return View(siparis);
            }
            
        }

        public ActionResult CariSec()
        {
            return View();
        }        
     

        public ActionResult CariOlustur()
        {
            return View();
        }


        public string CariDoldur(string CariKodu)
        {
            try
            {
                return JsonConvert.SerializeObject(MyBll.GetCari(CariKodu));
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public ActionResult KalemEkle(string id)
        {
            //string FatNo = (string)TempData["FatNo"];
            TempData["FatNo"] = id;

            //List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
            //TempData["Yaslandirma"] = yaslandirma;
            TempData["Depolar"] = MyBll.GetDepolar();

            string dt = MyBll.GetSiparisTarih(id).Date.ToShortDateString();
            TempData["Tarih"] = dt;

            var userID = User.Identity.Name;
            Kullanicilar kullanici = MyBll.GetUsers(userID);
            ViewBag.Kullanici = kullanici;
            return View();
        }

        [HttpPost]
        public string KalemEkle(string FatNo, string Tarih, string DepoKodu, string StokKodu, string StokIsim, string Miktar, string Fiyat)
        {

            try
            {
                MyBll.InsertKalemSQL_Satici(FatNo, Convert.ToDateTime(Tarih), StokKodu, Convert.ToInt32(DepoKodu), Convert.ToDecimal(Miktar.Replace(',', '.')), Convert.ToDecimal(Fiyat.Replace(',', '.')));

                string str = Url.Action("Details", new { id = FatNo });


                return (str);
            }
            catch (Exception exc)
            {

                return exc.Message ;
            }

        }

        public string IsExistControl(string id)
        {
            try
            {
                string FatNo = (string)TempData["FatNo"];
                TempData["FatNo"] = FatNo;
                List<SaticiSiparisKalemler> faturaKalemler = MyBll.GetSaticiSiparisKalemler(FatNo);
                int IsExist = faturaKalemler.Where(t => t.Sira == Convert.ToInt32(id)).First().HareketDurumu;

                if (IsExist == 0)
                {
                    MyBll.DeleteSaticiFatKalem(FatNo, id);
                    return "Silindi";
                }
                else
                {
                    return "İşlem görmüş bir kayıt silinemez.";
                }
            }
            catch
            {
                return "Hata";
            }

        }

        public string KalemKapat(string id)
        {
            try
            {
                string FatNo = (string)TempData["FatNo"];
                TempData["FatNo"] = FatNo;

                MyBll.UpdateHareketTur(FatNo, id, true);

                return "Kapandı";
            }
            catch (Exception exc)
            {
                string FatNo = (string)TempData["FatNo"];
                TempData["FatNo"] = FatNo;

                ModelState.AddModelError("", exc.Message);

                return "Hata :" + exc.Message;
            }

        }



        [ValidateInput(false)]
        public ActionResult GridViewPartialCariler()
        {
            var model = MyBll.GetCariler();
            return PartialView("_GridViewPartialCariler", model);
        }


        [ValidateInput(false)]
        public ActionResult GridViewPartialKalemler()
        {
            var FatNo = TempData["FatNo"].ToString();
            TempData["FatNo"] = FatNo;
            var model = MyBll.GetSaticiSiparisKalemler(FatNo);
            return PartialView("_GridViewPartialKalemler", model);
        }



        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = MyBll.GetSaticiSiparisler();
            return PartialView("_GridViewPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult GridLookupPartialStok()
        {
            var model = MyBll.GetStoklar(true);
            return PartialView("_GridLookupPartialStok", model);
        }

       

        public string BakiyeResult(string StokKodu)
        {
            return MyBll.GetBakiye(StokKodu);
        }

        public string GetAddedKalem(int id)
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            return JsonConvert.SerializeObject(model.Where(t => t.Sira == id).First());
        }

        public ActionResult PartialKalemAddMethod(int SiraNo,string DepoKodu, string StokKodu, string StokIsim, string Miktar, string Fiyat)
        {
            KalemViewModel kalemItem = new KalemViewModel();
            var model = (List<KalemViewModel>)TempData["Kalemler"];

            if (SiraNo != -1)
            {
                var kalem = model.Where(t => t.Sira == SiraNo).First();
                model.Remove(kalem);
                TempData["Kalemler"] = model;
            }

            int id = 0;



            if (model.Count > 0)
            {
                id = model.Last().Sira;
            }
            id++;

            

            kalemItem.Sira = id;
            kalemItem.StokKodu = StokKodu;
            kalemItem.StokIsim = StokIsim;

            kalemItem.DepoKodu = DepoKodu;
            kalemItem.Miktar = Convert.ToDecimal(Convert.ToDecimal(Miktar).ToString("#.##"));//.Replace(',','.'));
            kalemItem.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
            kalemItem.ToplamTutar = kalemItem.Miktar * kalemItem.Fiyat;
            kalemItem.ToplamMaliyetTutar = kalemItem.Miktar * kalemItem.MaliyetFiyat;
            kalemItem.IskontoluTutar = kalemItem.ToplamTutar - (kalemItem.IskontoOran/100 * kalemItem.ToplamTutar);

            model.Add(kalemItem);
            model.OrderBy(t => t.Sira);
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialSaticiSiparisKalem", model);
        }

        

        [ValidateInput(false)]
        public ActionResult GridViewPartialSaticiSiparisKalem()
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialSaticiSiparisKalem", model);
        }


        [ValidateInput(false)]
        public ActionResult GridViewPartialSaticiSiparisKalemDelete(int id)
        {

            var model = (List<KalemViewModel>)TempData["Kalemler"];
            KalemViewModel kalem = model.Where(t => t.Sira == id).First();
            model.Remove(kalem);
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialSaticiSiparisKalem", model);
        }
    }
}