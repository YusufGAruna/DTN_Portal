using DevExpress.Web.Mvc;
using Aruna_DTN_Entegrasyon.Filters;
using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Aruna_DTN_Entegrasyon.ViewModels;
using Newtonsoft.Json;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.Web;
using System.IO;
using System.Reflection;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    [Authorize]
    public class TeklifController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();
        Aruna_DTN_Entegrasyon.Reports.xrptTeklif rptTeklif = new Aruna_DTN_Entegrasyon.Reports.xrptTeklif();

        [Authorize]
        public ActionResult Index()
        {
            List<TeklifViewModel> teklifler = MyBll.GetTeklifler();
            return View(teklifler);
        }
        public ActionResult Create()
        {
            Teklifler teklif = new Teklifler();
            
            
            teklif.Tarih = DateTime.Now.Date;
            teklif.GecerlilikTarihi = DateTime.Now.Date.AddDays(7);
            TempData["Stoklar"] = MyBll.GetStoklar();
            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
            TempData["Yaslandirma"] = yaslandirma;

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
            TempData["Muhtelif"] = Parametreler.MuhtelifCari;

            var userID = User.Identity.Name;

            Kullanicilar kullanici = MyBll.GetUsers(userID);
            teklif.FatNo = MyBll.GetFatNo("Teklif", kullanici.UserName);
            teklif.Plasiyer = kullanici.UserName;
            ViewBag.Kullanici = kullanici;
            return View(teklif);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Teklifler teklif)
        {
            var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];

            var userID = User.Identity.Name;
            Kullanicilar kullanici = MyBll.GetUsers(userID);
            ViewBag.Kullanici = kullanici;

            try
            {
                

                if (string.IsNullOrEmpty(teklif.Cari.CariKod))
                {
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    //TempData["Kalemler"] = new List<KalemViewModel>();
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                    TempData["Kalemler"] = kalemModel;

                    

                    ModelState.AddModelError("", "Cari Bilgileri Eksik");
                    return View(teklif);
                }
                //var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];
                bool result = MyBll.InsertTeklif(teklif, kalemModel);
                if (!result)
                {
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    //TempData["Kalemler"] = new List<KalemViewModel>();
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                    TempData["Kalemler"] = kalemModel;

                    ModelState.AddModelError("", "Ekleme sırasında hata oluştu.");
                    return View(teklif);
                }

                TempData["Kalemler"] = new List<KalemViewModel>();
                return RedirectToAction("Index");
            }
            catch(Exception exc)
            {
                TempData["Stoklar"] = MyBll.GetStoklar();
                List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                TempData["Yaslandirma"] = yaslandirma;
                //TempData["Kalemler"] = new List<KalemViewModel>();
                TempData["Depolar"] = MyBll.GetDepolar();
                TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                TempData["Kalemler"] = kalemModel;

                ModelState.AddModelError("", "Ekleme sırasında hata oluştu." + Environment.NewLine + exc.Message);
                return View(teklif);
            }
        }

        

        public ActionResult CariSec()
        {
            return View();
        }
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeklifViewModel teklifler = MyBll.GetTeklifler().Where(t => t.FatNo == id).First();
            if (teklifler == null)

            {
                return HttpNotFound();
            }
            Teklifler teklif = new Teklifler();
            teklif.FatNo = teklifler.FatNo;
            teklif.Tarih = teklifler.Tarih;
            teklif.GecerlilikTarihi = teklifler.GecerlilikTarihi;
            teklif.Plasiyer = teklifler.Plasiyer;
            teklif.Plasiyer_Ad = teklifler.PlasiyerAd;
            teklif.AciklamaSahasi = teklifler.AciklamaSahasi;
            TempData["FatNo"] = teklifler.FatNo;
            List<TeklifKalemler> teklifKalemler = MyBll.GetTeklifKalemler(teklifler.FatNo);
            return View(teklifKalemler);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeklifViewModel teklifler = MyBll.GetTeklifler().Where(t => t.FatNo == id).First();
            


            if (teklifler == null)

            {
                return HttpNotFound();
            }

            Teklifler teklif = new Teklifler();

            teklif.FatNo = teklifler.FatNo;
            teklif.Tarih = teklifler.Tarih;
            teklif.GecerlilikTarihi = teklifler.GecerlilikTarihi;
            teklif.Plasiyer = teklifler.Plasiyer;
            teklif.Plasiyer_Ad = teklifler.PlasiyerAd;
            teklif.Cari_Kod = teklifler.CariKod;
            teklif.Cari_Isim = teklifler.CariIsim;
            teklif.AciklamaSahasi = teklifler.AciklamaSahasi;
            teklif.Cari = MyBll.GetMuhtelifCari(teklif.FatNo);

            TempData["Stoklar"] = MyBll.GetStoklar();
            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
            TempData["Yaslandirma"] = yaslandirma;
            TempData["Kalemler"] = new List<KalemViewModel>();
            TempData["Depolar"] = MyBll.GetDepolar();
            TempData["Muhtelif"] = Parametreler.MuhtelifCari;

            var userID = User.Identity.Name;

            Kullanicilar kullanici = MyBll.GetUsers(userID);

            ViewBag.Kullanici = kullanici;

            List<TeklifKalemler> kalemler = MyBll.GetTeklifKalemler(teklif.FatNo);
            List<KalemViewModel> kalemView = new List<KalemViewModel>();
            foreach (TeklifKalemler teklifKalem in kalemler)
            {
                KalemViewModel kalem = new KalemViewModel();
                
                kalem.Sira = teklifKalem.Sira;
                kalem.StokKodu = teklifKalem.StokKodu;
                kalem.StokIsim = teklifKalem.StokAd;
                kalem.Miktar = teklifKalem.Miktar;
                kalem.Fiyat = teklifKalem.Fiyat;
                kalem.MaliyetFiyat = teklifKalem.MaliyetFiyat;
                kalem.IskontoOran = teklifKalem.IskontoOran;

                kalem.IskontoluTutar = teklifKalem.IskontoluTutar;
                kalem.ToplamTutar = teklifKalem.ToplamTutar;
                kalem.ToplamMaliyetTutar = teklifKalem.ToplamMaliyetTutar;
                kalem.DepoKodu = teklifKalem.DepoKodu.ToString();
                kalem.DepoAdi = teklifKalem.DepoAdi;
                kalem.ToplamKDVliTutar = MyBll.GetKDVOrani(teklifKalem.StokKodu);
                kalem.Bakiye = teklifKalem.Bakiye;

                kalemView.Add(kalem);

            }
            TempData["Kalemler"] = kalemView;


            return View(teklif);
        }
        [HttpPost]
        public ActionResult Edit(Teklifler teklif)
        {
            var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];
            try
            {

                var userID = User.Identity.Name;
                Kullanicilar kullanici = MyBll.GetUsers(userID);
                ViewBag.Kullanici = kullanici;

                if (string.IsNullOrEmpty(teklif.Cari.CariKod))
                {
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    //TempData["Kalemler"] = new List<KalemViewModel>();
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                    TempData["Kalemler"] = kalemModel;

                    //edit de bazı durumlarda kullanıcı null geldiği için en yukarı çekildi
                    //var userID = User.Identity.Name;
                    //Kullanicilar kullanici = MyBll.GetUsers(userID);
                    //ViewBag.Kullanici = kullanici;

                    ModelState.AddModelError("", "Cari Bilgileri Eksik");
                    return View(teklif);
                }
                //var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];
                bool result = MyBll.UpdateTeklif(teklif, kalemModel);
                if (!result)
                {
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    //TempData["Kalemler"] = new List<KalemViewModel>();
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                    TempData["Kalemler"] = kalemModel;

                    ModelState.AddModelError("", "Ekleme sırasında hata oluştu.");
                    return View(teklif);
                }
                TempData["Kalemler"] = new List<KalemViewModel>();
                return RedirectToAction("Index");
            }
            catch(Exception exc)
            {
                TempData["Stoklar"] = MyBll.GetStoklar();
                List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                TempData["Yaslandirma"] = yaslandirma;
                //TempData["Kalemler"] = new List<KalemViewModel>();
                TempData["Depolar"] = MyBll.GetDepolar();
                TempData["Muhtelif"] = Parametreler.MuhtelifCari;
                TempData["Kalemler"] = kalemModel;

                ModelState.AddModelError("", "Ekleme sırasında hata oluştu." + Environment.NewLine + exc.Message);
                return View(teklif);
            }
            
        }

        public ActionResult PrintTeklif(string id)
        {
            
            List<TeklifPrint> _rptSource = new List<TeklifPrint>();
            TeklifPrint _tkl = new TeklifPrint();
            TeklifViewModel _TeklifUst = MyBll.GetTeklifler().Where(t => t.FatNo == id).First();

            _tkl.AciklamaSahasi = _TeklifUst.AciklamaSahasi;
            _tkl.Cari_Isim =  _TeklifUst.CariIsim;
            _tkl.Cari_Kod =  _TeklifUst.CariKod;
            _tkl.FatNo = _TeklifUst.FatNo;
            _tkl.GecerlilikTarihi = _TeklifUst.GecerlilikTarihi;
            _tkl.Plasiyer =  _TeklifUst.Plasiyer;
            _tkl.Plasiyer_Ad = _TeklifUst.PlasiyerAd;
            _tkl.Tarih = _TeklifUst.Tarih;
            _tkl.SevkAdresi = _TeklifUst.SevkAdresi;

            _tkl.TeklifKalemleri = new List<TeklifPrintKalem>();
            List<TeklifKalemler> _lst =  MyBll.GetTeklifKalemler(id);
            foreach (TeklifKalemler _klm in _lst)
            {
                TeklifPrintKalem tklm = new TeklifPrintKalem();
                tklm.Birim = _klm.Birim;
                tklm.DepoAdi = _klm.DepoAdi;
                tklm.DepoKodu = _klm.DepoKodu;
                tklm.Fiyat = _klm.Fiyat;
                tklm.IskontoluFiyat = _klm.Fiyat * (100 - _klm.IskontoOran) / 100;
                tklm.IskontoluTutar = _klm.IskontoluTutar;
                tklm.IskontoOran = _klm.IskontoOran;
                tklm.Miktar = _klm.Miktar;
                tklm.Sira = _klm.Sira;
                tklm.StokAd = _klm.StokAd;
                tklm.StokKodu = _klm.StokKodu;
                tklm.ToplamTutar = _klm.ToplamTutar;
                tklm.KdvTutari = _klm.KdvOrani * (_klm.ToplamTutar / (100 + _klm.KdvOrani));
                _tkl.TeklifKalemleri.Add(tklm);
            }

            //toplamlar hesaplanacak
            _tkl.Brut_Tutar = _tkl.TeklifKalemleri.Sum(t => t.ToplamTutar);
            _tkl.Genel_Toplam = _tkl.TeklifKalemleri.Sum(t => t.IskontoluTutar);
            _tkl.Isk_Tutar = _tkl.Brut_Tutar - _tkl.Genel_Toplam;
            _tkl.Kdv = _tkl.TeklifKalemleri.Sum(t => t.KdvTutari);
            _tkl.KDVSizToplam = _tkl.Genel_Toplam - _tkl.Kdv;

            DateTime Today = DateTime.Now;

            _rptSource.Add(_tkl);

            string FileName = "Teklif_" + _tkl.Cari_Isim.Replace('.','_') + "_" + Today.Year + "_" + Today.Month + "_" + Today.Day + "_" + Today.Hour + "_" + Today.Minute;

            rptTeklif.DataSourceDemanded += (s, e) =>
            {
                ((Aruna_DTN_Entegrasyon.Reports.xrptTeklif)s).DataSource = _rptSource;
                ((Aruna_DTN_Entegrasyon.Reports.xrptTeklif)s).Name = FileName;
            };
            //string Path = Parametreler.FilePath + @"\Gidenler\"+ FileName + ".pdf";
            //Path = Path.Replace(@"file:\", "");
            //rptTeklif.ExportToPdf(Path);
            //List<string> pdfs = new List<string>();
            //pdfs.Add(Path);
            //Aruna.Utilities.Utilities util = new Aruna.Utilities.Utilities();
            //string Eposta = MyBll.GetCariPosta(_tkl.Cari_Kod, _tkl.FatNo);

            //if (!string.IsNullOrEmpty(Eposta))
            //{
            //    util.SendMail("mail.dtninsaat.com.tr", 443, false, "netsis@dtninsaat.com.tr", Eposta , "", "netsis@dtninsaat.com.tr", "Netsis19**", "DTN TEKLIF FORMU", "Teklifimiz ektedir.", true, pdfs);
            //}
            
            return View("PrintTeklif",rptTeklif);
            
        }

        public string SendMail(string id)
        {

            List<TeklifPrint> _rptSource = new List<TeklifPrint>();
            TeklifPrint _tkl = new TeklifPrint();
            TeklifViewModel _TeklifUst = MyBll.GetTeklifler().Where(t => t.FatNo == id).First();

            _tkl.AciklamaSahasi = _TeklifUst.AciklamaSahasi;
            _tkl.Cari_Isim = _TeklifUst.CariIsim;
            _tkl.Cari_Kod = _TeklifUst.CariKod;
            _tkl.FatNo = _TeklifUst.FatNo;
            _tkl.GecerlilikTarihi = _TeklifUst.GecerlilikTarihi;
            _tkl.Plasiyer = _TeklifUst.Plasiyer;
            _tkl.Plasiyer_Ad = _TeklifUst.PlasiyerAd;
            _tkl.Tarih = _TeklifUst.Tarih;
            _tkl.SevkAdresi = _TeklifUst.SevkAdresi;

            _tkl.TeklifKalemleri = new List<TeklifPrintKalem>();
            List<TeklifKalemler> _lst = MyBll.GetTeklifKalemler(id);
            foreach (TeklifKalemler _klm in _lst)
            {
                TeklifPrintKalem tklm = new TeklifPrintKalem();
                tklm.Birim = _klm.Birim;
                tklm.DepoAdi = _klm.DepoAdi;
                tklm.DepoKodu = _klm.DepoKodu;
                tklm.Fiyat = _klm.Fiyat;
                tklm.IskontoluFiyat = _klm.Fiyat * (100 - _klm.IskontoOran) / 100;
                tklm.IskontoluTutar = _klm.IskontoluTutar;
                tklm.IskontoOran = _klm.IskontoOran;
                tklm.Miktar = _klm.Miktar;
                tklm.Sira = _klm.Sira;
                tklm.StokAd = _klm.StokAd;
                tklm.StokKodu = _klm.StokKodu;
                tklm.ToplamTutar = _klm.ToplamTutar;
                tklm.KdvTutari = _klm.KdvOrani * (_klm.ToplamTutar / (100 + _klm.KdvOrani));
                _tkl.TeklifKalemleri.Add(tklm);
            }

            //toplamlar hesaplanacak
            _tkl.Brut_Tutar = _tkl.TeklifKalemleri.Sum(t => t.ToplamTutar);
            _tkl.Genel_Toplam = _tkl.TeklifKalemleri.Sum(t => t.IskontoluTutar);
            _tkl.Isk_Tutar = _tkl.Brut_Tutar - _tkl.Genel_Toplam;
            _tkl.Kdv = _tkl.TeklifKalemleri.Sum(t => t.KdvTutari);
            _tkl.KDVSizToplam = _tkl.Genel_Toplam - _tkl.Kdv;

            DateTime Today = DateTime.Now;

            _rptSource.Add(_tkl);

            string FileName = "Teklif_" + _tkl.Cari_Isim.Replace('.', '_') + "_" + Today.Year + "_" + Today.Month + "_" + Today.Day + "_" + Today.Hour + "_" + Today.Minute;

            rptTeklif.DataSourceDemanded += (s, e) =>
            {
                ((Aruna_DTN_Entegrasyon.Reports.xrptTeklif)s).DataSource = _rptSource;
                ((Aruna_DTN_Entegrasyon.Reports.xrptTeklif)s).Name = FileName;
            };

            try
            {
                string Path = Parametreler.FilePath + @"\Gidenler\" + FileName + ".pdf";
                Path = Path.Replace(@"file:\", "");
                rptTeklif.ExportToPdf(Path);
                List<string> pdfs = new List<string>();
                pdfs.Add(Path);
                Aruna.Utilities.Utilities util = new Aruna.Utilities.Utilities();
                string Eposta = MyBll.GetCariPosta(_tkl.Cari_Kod, _tkl.FatNo);

                if (!string.IsNullOrEmpty(Eposta))
                {
                    util.SendMail("mail.dtninsaat.com.tr", 443, false, "netsis@dtninsaat.com.tr", Eposta, "", "netsis@dtninsaat.com.tr", "Netsis19**", "DTN TEKLIF FORMU", "Teklifimiz ektedir.", true, pdfs);
                }

                return "Mail Gönderildi";
            }
            catch(Exception exc)
            {
                return exc.Message;

                
            }
               

        }




        public ActionResult CariOlustur()
        {
            return View();
        }

        public string GrupKoduKontrol(string GrupKodu)


        {
            return JsonConvert.SerializeObject(MyBll.GrupCheck(GrupKodu));
        }
       

        //public ActionResult 


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

        public string StokValues(string StokKodu)
        {
            return JsonConvert.SerializeObject(MyBll.GetStoklar().Where(t => t.StokKodu == StokKodu).First());
        }

        public ActionResult ToSiparis(object[] kalemler)
        {
            var FatNo = TempData["FatNo"].ToString();
            TempData["FatNo"] = FatNo;
            TeklifViewModel teklifler = MyBll.GetTeklifler().Where(t => t.FatNo == FatNo).First();
            List<TeklifKalemler> teklifKalemler = MyBll.GetTeklifKalemler(teklifler.FatNo);
            return RedirectToAction("Create", "Siparis", new { area = ""});
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
            var model = MyBll.GetTeklifKalemler(FatNo);
            return PartialView("_GridViewPartialKalemler", model);
        }



        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = MyBll.GetSiparisler();
            return PartialView("_GridViewPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult GridLookupPartialStok()
        {
            var model = MyBll.GetStoklar();
            return PartialView("_GridLookupPartialStok", model);
        }

        public ActionResult PartialYaslandirmaMethod(string StokKodu)
        {
            List<YaslandirmaViewModel> model = model = MyBll.GetYaslandirma(StokKodu);
            TempData["Yaslandirma"] = model;
            var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = kalemler;
            return PartialView("_GridLookupPartialYaslandirma", model);
        }

        public string BakiyeResult(string StokKodu)
        {
            return MyBll.GetBakiye(StokKodu);
        }

        public string GetAddedKalem(int id)
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            var stokModel = model.Where(t => t.Sira == id).First();
            TempData["Yaslandirma"] = MyBll.GetYaslandirma(stokModel.StokKodu);
            return JsonConvert.SerializeObject(model.Where(t => t.Sira == id).First());
        }

        public string PartialKalemAddMethod(int SiraNo, string StokKodu,string StokIsim,string Depo,string DepoAdi, string Miktar, string Fiyat, string MaliyetFiyat, string IskontoOran, string Kdv, string Bakiye)
        {
            
                KalemViewModel kalemItem = new KalemViewModel();
                var model = (List<KalemViewModel>)TempData["Kalemler"];
            string _result = string.Empty;
            try
            {
                var Depolar = MyBll.GetYaslandirma(StokKodu);
                //var MaxMiktar = Depolar.Where(t => t.Fiyat == Convert.ToDecimal(MaliyetFiyat) && t.DepoKodu == Depo).First();
                decimal MaxMiktar = 0;
                if (Depolar.Any(t => t.Fiyat == Convert.ToDecimal(MaliyetFiyat) && t.DepoKodu == Depo))
                {
                    MaxMiktar = Depolar.First(t => t.Fiyat == Convert.ToDecimal(MaliyetFiyat) && t.DepoKodu == Depo).Miktar;
                }
                decimal totalMiktar = 0;
                if (MaliyetFiyat != "")
                {
                     totalMiktar = model.Where(t => t.StokKodu == StokKodu && t.DepoKodu == Depo && t.MaliyetFiyat == Convert.ToDecimal(MaliyetFiyat)).Select(t => t.Miktar).Sum();
                }
                else
                {
                    MaliyetFiyat = "0";
                }

                //if (totalMiktar + Convert.ToDecimal(Miktar) > MaxMiktar)
                ////if (totalMiktar + Convert.ToDecimal(Miktar) > MaxMiktar.Miktar)
                //{
                //    TempData["Kalemler"] = model;
                //    return "Depoda yeterli ürün yok lütfen Satıcı Siparişi girin";
                //}
                //else
                //{


                    if (SiraNo != -1)
                    {
                        var UpdateItem = model.Where(t => t.Sira == SiraNo).First();
                        model.Remove(UpdateItem);

                        kalemItem.StokKodu = StokKodu;
                        kalemItem.StokIsim = StokIsim;
                        kalemItem.Sira = SiraNo; //06.06.2021,uu
                        kalemItem.DepoKodu = Depo;
                        kalemItem.DepoAdi = DepoAdi;
                        kalemItem.Miktar = Convert.ToDecimal(Miktar);//.Replace(',','.'));
                        kalemItem.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
                        kalemItem.MaliyetFiyat = Convert.ToDecimal(MaliyetFiyat);//.Replace(',', '.'));
                        kalemItem.ToplamTutar = kalemItem.Miktar * kalemItem.Fiyat;
                        kalemItem.ToplamMaliyetTutar = kalemItem.Miktar * kalemItem.MaliyetFiyat;
                        kalemItem.IskontoOran = Convert.ToDecimal(IskontoOran);//.Replace(',', '.'));
                        kalemItem.IskontoluTutar = kalemItem.ToplamTutar - (kalemItem.IskontoOran / 100 * kalemItem.ToplamTutar);
                        kalemItem.ToplamKDVliTutar = kalemItem.ToplamTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.ToplamTutar);
                        kalemItem.Bakiye = Convert.ToDecimal(Bakiye);//.Replace(',', '.'));

                        model.Add(kalemItem);
                    }
                    else
                    {
                        var newItem = model.Where(t => t.StokKodu == StokKodu && t.DepoKodu == Depo && t.MaliyetFiyat == Convert.ToDecimal(MaliyetFiyat) && t.IskontoOran == Convert.ToDecimal(IskontoOran));

                        if (newItem.Count() > 0)
                        {
                            KalemViewModel modelFirst = newItem.First();

                            model.Remove(modelFirst);

                            modelFirst.Miktar += Convert.ToDecimal(Miktar);//.Replace(',','.'));
                            modelFirst.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
                            modelFirst.ToplamTutar = modelFirst.Miktar * modelFirst.Fiyat;
                            modelFirst.ToplamMaliyetTutar = modelFirst.Miktar * modelFirst.MaliyetFiyat;
                            modelFirst.IskontoluTutar = modelFirst.ToplamTutar - (modelFirst.IskontoOran / 100 * modelFirst.ToplamTutar);
                            kalemItem.ToplamKDVliTutar = kalemItem.ToplamTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.ToplamTutar);
                            model.Add(modelFirst);
                        }
                        else
                        {
                            int id = 0;

                            if (model.Count > 0)
                            {
                                id = model.Last().Sira;
                            }
                            id++;



                            kalemItem.Sira = id;
                            kalemItem.StokKodu = StokKodu;
                            kalemItem.StokIsim = StokIsim;

                            kalemItem.DepoKodu = Depo;
                            kalemItem.DepoAdi = DepoAdi;
                            kalemItem.Miktar = Convert.ToDecimal(Miktar);//.Replace(',','.'));
                            kalemItem.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
                            kalemItem.MaliyetFiyat = Convert.ToDecimal(MaliyetFiyat);//.Replace(',', '.'));
                            kalemItem.ToplamTutar = kalemItem.Miktar * kalemItem.Fiyat;
                            kalemItem.ToplamMaliyetTutar = kalemItem.Miktar * kalemItem.MaliyetFiyat;
                            kalemItem.IskontoOran = Convert.ToDecimal(IskontoOran);//.Replace(',', '.'));
                            kalemItem.IskontoluTutar = kalemItem.ToplamTutar - (kalemItem.IskontoOran / 100 * kalemItem.ToplamTutar);
                            kalemItem.ToplamKDVliTutar = kalemItem.ToplamTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.ToplamTutar);
                            kalemItem.Bakiye = Convert.ToDecimal(Bakiye);//.Replace(',', '.'));

                            model.Add(kalemItem);
                        }
                    }
                //}
                model.OrderBy(t => t.Sira);
                _result = "Kalem Ekleme Başarılı";
            }
            catch(Exception exc)
            {
                model.OrderBy(t => t.Sira);
                _result = "Kalem eklemede hata: " + exc.Message;
            }
            TempData["Kalemler"] = model;
            return _result;
        }

        [ValidateInput(false)]
        public ActionResult GridLookupPartialYaslandirma()
        {
            var model = TempData["Yaslandirma"];
            TempData["Yaslandirma"] = model;
            var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = kalemler;
            return PartialView("_GridLookupPartialYaslandirma", model);



        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialSiparisKalem()
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialSiparisKalem", model);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialTeklifUst()
        {
            var model = MyBll.GetTeklifler();
            return PartialView("_GridViewPartialTeklifUst", model);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialTeklifKalemler()
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialTeklifKalemler", model);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialTeklifKalemDelete(int id)
        {

            var model = (List<KalemViewModel>)TempData["Kalemler"];
            KalemViewModel kalem = model.Where(t => t.Sira == id).First();
            model.Remove(kalem);
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialTeklifKalemler", model);
        }


      

       
    }
}