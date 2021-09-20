using DevExpress.Web.Mvc;
using Aruna_DTN_Entegrasyon.Filters;
using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Aruna_DTN_Entegrasyon.ViewModels;
using Newtonsoft.Json;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    
    public class SiparisController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();
        Aruna_DTN_Entegrasyon.Reports.xptSiparisReport rptSiparis = new Aruna_DTN_Entegrasyon.Reports.xptSiparisReport();

        [Authorize]
        public ActionResult Index()
        {
            List<Siparisler> siparisler = MyBll.GetSiparisler();
            TempData["Kalemler"] = new List<KalemViewModel>();
            return View(siparisler);
        }
        [Authorize]
        public ActionResult Create(string TekNo = null, List<string> SelectedRows = null)
        {
            
           
            Siparisler siparis = new Siparisler();
            var userID = User.Identity.Name;
            siparis.TeklifNo = TekNo;
            Kullanicilar kullanici = MyBll.GetUsers(userID);
            siparis.FatNo = MyBll.GetFatNo("Siparis", kullanici.UserName);
            siparis.Plasiyer = kullanici.UserName;
            siparis.Tarih = DateTime.Today;
            siparis.OdemeTarih = DateTime.Today;
            TempData["Stoklar"] = MyBll.GetStoklar();
            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
            TempData["Yaslandirma"] = yaslandirma;
            TempData["Depolar"] = MyBll.GetDepolar();
            if (!string.IsNullOrEmpty(TekNo))
            {
                TeklifViewModel teklif = MyBll.GetTeklif(TekNo);
                siparis.AciklamaSahasi = teklif.AciklamaSahasi;
                Cariler cari = MyBll.GetMuhtelifCari(TekNo);
                TempData["TekNo"] = TekNo;
                TempData["SelectedRows"] = SelectedRows;
                siparis.Cari = cari;
                if(teklif.CariKod == Parametreler.MuhtelifCari)
                {
                    siparis.Cari.CariKod = string.Empty;
                    ModelState.AddModelError("", "Yeni Cari Oluşturmanız gerekmekte Cariniz Muhteliftir");
                }
                List<TeklifKalemler> kalemler = MyBll.GetTeklifKalemler(TekNo);

                
                    List<KalemViewModel> kalemView = new List<KalemViewModel>();
                    foreach (string SiraNo in SelectedRows)
                    {
                        KalemViewModel kalem = new KalemViewModel();
                        TeklifKalemler teklifKalem = kalemler.Where(t => t.Sira == Convert.ToInt32(SiraNo)).First();
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
                        kalem.Bakiye = teklifKalem.Bakiye;

                        kalemView.Add(kalem);

                    }
                    TempData["Kalemler"] = kalemView;
                
                
                
            }
            else
            {
                if(TempData["Kalemler"] != null)
                {
                    var kalems = TempData["Kalemler"];
                    TempData["Kalemler"] = kalems;
                }
                else
                {
                    TempData["Kalemler"] = new List<KalemViewModel>();
                }

                siparis.Cari = new Cariler();

            }
            
            ViewBag.Kullanici = kullanici;
            return View(siparis);
        }

        

        [HttpPost]
        public ActionResult Create(Siparisler siparis)
        {
            var kalemModel = (List<KalemViewModel>)TempData["Kalemler"];
            try
            {
                var control = kalemModel.Where(t => t.Tip == null).ToList();

                if (control.Count > 0 || siparis.SipTip == null)
                {
                    var userID = User.Identity.Name;
                    Kullanicilar kullanici = MyBll.GetUsers(userID);
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Kalemler"] = kalemModel;
                    ViewBag.Kullanici = kullanici;
                    if (control.Count > 0)
                    {
                        ModelState.AddModelError("", "Kalemlerin Tipi Boş olamaz");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sipariş Tipi Boş olamaz");
                    }

                    return View(siparis);
                }

                if (string.IsNullOrEmpty(siparis.Cari.CariKod) || siparis.Cari.CariKod == Parametreler.MuhtelifCari)
                {
                    var userID = User.Identity.Name;
                    Kullanicilar kullanici = MyBll.GetUsers(userID);
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Kalemler"] = kalemModel;
                    ViewBag.Kullanici = kullanici;
                    ModelState.AddModelError("", "Lütfen Cari Giriniz");

                    return View(siparis);
                }

                if(!MyBll.GetCariIfExist(siparis.Cari.CariKod))
                {
                    var CariRes = MyBll.CariKart(siparis.Cari);

                    if (!CariRes.IsSuccessful)
                    {
                        var userID = User.Identity.Name;
                        Kullanicilar kullanici = MyBll.GetUsers(userID);
                        TempData["Stoklar"] = MyBll.GetStoklar();
                        List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                        TempData["Yaslandirma"] = yaslandirma;
                        TempData["Depolar"] = MyBll.GetDepolar();
                        TempData["Kalemler"] = kalemModel;
                        ViewBag.Kullanici = kullanici;
                        ModelState.AddModelError("", "Cari Kart oluştururken hata oluştu." + Environment.NewLine + CariRes.ErrorDesc);

                        return View(siparis);
                    }
                }
                

                


                var datkalems = kalemModel.Where(t => t.Tip == FatType.Bloke).ToList();
                var ambarkalems = kalemModel.Where(t => t.Tip == FatType.OnFatura).ToList();
                var transitkalems = kalemModel.Where(t => t.Tip == FatType.Transit).ToList();




                var result = MyBll.MusteriSiparis(siparis, kalemModel);
                if (!result.IsSuccessful)
                {
                    var userID = User.Identity.Name;
                    Kullanicilar kullanici = MyBll.GetUsers(userID);
                    TempData["Stoklar"] = MyBll.GetStoklar();
                    List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                    TempData["Yaslandirma"] = yaslandirma;
                    TempData["Depolar"] = MyBll.GetDepolar();
                    TempData["Kalemler"] = kalemModel;
                    ViewBag.Kullanici = kullanici;
                    ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + result.ErrorDesc);

                    return View(siparis);
                }
                else
                {
                    if (datkalems.Count > 0)
                    {
                        var resultDat = MyBll.MusteriSiparisDAT(siparis, datkalems);

                        if (!resultDat.IsSuccessful)
                        {

                            MyBll.DeleteFat(siparis.FatNo, "ftSSip", siparis.Cari.CariKod);

                            var userID = User.Identity.Name;
                            Kullanicilar kullanici = MyBll.GetUsers(userID);
                            TempData["Stoklar"] = MyBll.GetStoklar();
                            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                            TempData["Yaslandirma"] = yaslandirma;
                            TempData["Depolar"] = MyBll.GetDepolar();
                            TempData["Kalemler"] = kalemModel;
                            ViewBag.Kullanici = kullanici;
                            ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + resultDat.ErrorDesc);

                            return View(siparis);
                        }
                    }


                    if (ambarkalems.Count > 0)
                    {
                        var resultAmbar = MyBll.MusteriSiparisAmbar(siparis, ambarkalems);

                        if (!resultAmbar.IsSuccessful)
                        {

                            MyBll.DeleteFat(siparis.FatNo, "ftSSip", siparis.Cari.CariKod);

                            if(datkalems.Count > 0)
                            {
                                MyBll.DeleteFat(siparis.FatNo, "ftLokalDepo", siparis.Cari.CariKod);
                            }
                            

                            var userID = User.Identity.Name;
                            Kullanicilar kullanici = MyBll.GetUsers(userID);
                            TempData["Stoklar"] = MyBll.GetStoklar();
                            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                            TempData["Yaslandirma"] = yaslandirma;
                            TempData["Depolar"] = MyBll.GetDepolar();
                            TempData["Kalemler"] = kalemModel;
                            ViewBag.Kullanici = kullanici;
                            ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + resultAmbar.ErrorDesc);

                            return View(siparis);
                        }
                    }
                }


                TempData["Kalemler"] = new List<KalemViewModel>();
                return RedirectToAction("Index");
            }
            catch(Exception exc)
            {
                var userID = User.Identity.Name;
                Kullanicilar kullanici = MyBll.GetUsers(userID);
                TempData["Stoklar"] = MyBll.GetStoklar();
                List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                TempData["Yaslandirma"] = yaslandirma;
                TempData["Depolar"] = MyBll.GetDepolar();
                TempData["Kalemler"] = kalemModel;
                ViewBag.Kullanici = kullanici;
                ModelState.AddModelError("", "Sipariş Oluştururken hata oluştu." + Environment.NewLine + exc.Message);
                return View(siparis);
            }
            
        }

        public ActionResult CariSec()
        {
            return View();
        }
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Siparisler siparisler = MyBll.GetSiparisler().Where(t => t.FatNo == id).First();
            if (siparisler == null)

            {
                return HttpNotFound();
            }
            TempData["FatNo"] = siparisler.FatNo;
            List<SiparisKalemler> faturaKalemler = MyBll.GetSiparisKalemler(siparisler.FatNo);
            return View(model:id);
        }

        public ActionResult CariOlustur(Cariler cari = null, Siparisler siparis = null)
        {
            var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = kalemler;
            TempData["Siparis"] = siparis;

            if (TempData["TekNo"] != null)
            {
                var tekno = TempData["TekNo"];
                var Select = TempData["SelectedRows"];
                TempData["SelectedRows"] = Select;
                TempData["TekNo"] = tekno;
            }
            

            if (cari == null)
            {
                //TempData["Muhtelif"] = false;
                return View();
            }
            else
            {
                if(cari.CariKod == Parametreler.MuhtelifCari)
                {
                    //cari.CariKod = null;
                    return View(cari);
                }
                else
                {
                    //TempData["Muhtelif"] = false;
                    return View();
                }
            }
            
        }

        public ActionResult BlokeCoz(List<string> SelectedRows)
        {
            TempData["SelectedBlokeler"] = SelectedRows;
            
            return View();
        }

        public ActionResult Bloke()
        {
            List<BlokeCozViewModel> blokeler = MyBll.GetBlokeler();
            return View(blokeler);
        }

        public ActionResult BlokeInsert(List<string> ItemList)
        {
            List<string> blokeler = (List<string>)TempData["SelectedBlokeler"];
            try
            {
                List<BlokeCozViewModel> blokeListesi = new List<BlokeCozViewModel>();
                int i = 0;
                foreach (string s in blokeler)
                {
                    blokeListesi.Add(MyBll.GetBlokeler().First(t => t.ID == Convert.ToInt32(s)));
                    blokeListesi[i].ID = i+1;
                    i++;
                }

                i = 0;
                foreach (string s in ItemList)
                {
                    
                    blokeListesi[i].BlokeliMiktar = decimal.Parse(s.Replace(',','.'), CultureInfo.InvariantCulture);
                    i++;
                }

                MyBll.InsertSevkEmri(blokeListesi);


                return RedirectToAction("Bloke");
            }
            catch(Exception exc)
            {
                string Hata = exc.Message;
                if (exc.InnerException != null)
                {
                    Hata = Hata + Environment.NewLine + exc.InnerException;
                }

                return View("BlokeCoz",blokeler);
            }
            
        }

        public ActionResult KalemEkle(string id)
        {
            //string FatNo = (string)TempData["FatNo"];
            TempData["FatNo"] = id;

            List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
            TempData["Yaslandirma"] = yaslandirma;
            TempData["Depolar"] = MyBll.GetDepolar();

            string dt = MyBll.GetSiparisTarih(id).Date.ToShortDateString();
            TempData["Tarih"] = dt;

            var userID = User.Identity.Name;
            Kullanicilar kullanici = MyBll.GetUsers(userID);
            ViewBag.Kullanici = kullanici;
            return View();
        }

        [HttpPost]
        public string KalemEkle(string FatNo,string Tarih, int Tip, string StokKodu, string StokIsim, string Depo, string DepoAdi, string Miktar, string Fiyat, string MaliyetFiyat, string IskontoOran, string Bakiye)
        {
            //string FatNo = (string)TempData["FatNo"];
            //TempData["FatNo"] = FatNo;

            try
            {
                //string Fiy = Fiyat.Replace(".", "");
                MyBll.InsertKalemSQL(FatNo,Convert.ToDateTime(Tarih), Tip, StokKodu, Convert.ToInt32(Depo), Convert.ToDecimal(Miktar.Replace(',', '.')), Convert.ToDecimal(MaliyetFiyat.Replace(',','.')), Convert.ToDecimal(Fiyat.Replace(',','.')), Convert.ToDecimal(IskontoOran));

                string str = Url.Action("Details", new { id = FatNo });


                return (str);

                //return RedirectToAction("Details", new { id = FatNo });
            }
            catch(Exception exc)
            {
                //TempData["FatNo"] = FatNo;

                //List<YaslandirmaViewModel> yaslandirma = new List<YaslandirmaViewModel>();
                //TempData["Yaslandirma"] = yaslandirma;
                //TempData["Depolar"] = MyBll.GetDepolar();

                //string dt = MyBll.GetSiparisTarih(FatNo).Date.ToShortDateString();
                //TempData["Tarih"] = dt;

                //var userID = User.Identity.Name;
                //Kullanicilar kullanici = MyBll.GetUsers(userID);
                //ViewBag.Kullanici = kullanici;

                return exc.Message;
            }
            
        }

        [HttpPost]
        public ActionResult CariOlustur(Cariler cari)
        {


            if(string.IsNullOrEmpty(cari.CariKod))
            {
                ModelState.AddModelError("", "Lütfen Cari Kodu Giriniz");
                return View(cari);
            }
            else
            {
                var res = MyBll.CariKart(cari);

                if (res.IsSuccessful)
                {
                    if (TempData["TekNo"] != null)
                    {
                        return RedirectToAction("Create", new { TekNo = TempData["TekNo"], SelectedRows = TempData["SelectedRows"] });
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    if(TempData["TekNo"] != null)
                    {
                        var tekno = TempData["TekNo"];
                        var Select = TempData["SelectedRows"];
                        TempData["SelectedRows"] = Select;
                        TempData["TekNo"] = tekno;
                    }
                    

                    ModelState.AddModelError("", res.ErrorDesc);

                    return View(cari);
                }
            }
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

        public string GrupKoduKontrol(string GrupKodu)
        {
            return JsonConvert.SerializeObject(MyBll.GrupCheck(GrupKodu));
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
            var model = MyBll.GetSiparisKalemler(FatNo);
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
            var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = kalemler;
            var model = MyBll.GetStoklar();
            return PartialView("_GridLookupPartialStok", model);
        }

        public ActionResult PartialYaslandirmaMethod(string StokKodu)
        {
            List<YaslandirmaViewModel> model =  MyBll.GetYaslandirma(StokKodu);
            TempData["Yaslandirma"] = model;
            var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = kalemler;
            return PartialView("_GridLookupPartialYaslandirma", model);
        }

        public string BakiyeResult(string StokKodu)
        {
            return MyBll.GetBakiye(StokKodu);
        }

        public string StokValues(string StokKodu)
        {
            try
            {
                var kalemler = (List<KalemViewModel>)TempData["Kalemler"];
                TempData["Kalemler"] = kalemler;
                return JsonConvert.SerializeObject(MyBll.GetStoklar().Where(t => t.StokKodu == StokKodu).First());
            }
            catch
            {
                return "Hata";
            }
            
        }

        public string IsExistControl(string id)
        {
            try
            {
                string FatNo = (string)TempData["FatNo"];
                TempData["FatNo"] = FatNo;
                List<SiparisKalemler> faturaKalemler = MyBll.GetSiparisKalemler(FatNo);
                int IsExist = faturaKalemler.Where(t => t.Sira == Convert.ToInt32(id)).First().IsExist;

                if(IsExist == 0)
                {
                    MyBll.DeleteFatKalem(FatNo, id);
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
                
                List<SiparisKalemler> faturaKalemler = MyBll.GetSiparisKalemler(FatNo);
                
                

                decimal mik = MyBll.GetBlokeliMiktar(FatNo, Convert.ToInt32(id));

                if (mik > 0)
                {
                    var res = MyBll.DATKapatma(faturaKalemler.Where(t => t.Sira == Convert.ToInt32(id)).ToList(), mik);
                    if (!res.IsSuccessful)
                    {
                        throw new Exception(res.ErrorDesc);
                    }
                }

                MyBll.UpdateHareketTur(FatNo, id, false);

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


        public string GetAddedKalem(int id)
        {
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            TempData["Kalemler"] = model;
            //var stokModel = model.Where(t => t.Sira == id).First();
            //TempData["Yaslandirma"] = MyBll.GetYaslandirma(stokModel.StokKodu);
            //PartialYaslandirmaMethod(stokModel.StokKodu);
            return JsonConvert.SerializeObject(model.Where(t => t.Sira == id).First());
        }
        

        //SIRA NO -1 se yeni kayıt SIRA NO gelirse update

        public string PartialKalemAddMethod(int Tip,int SiraNo, string StokKodu, string StokIsim, string Depo, string DepoAdi, string Miktar, string Fiyat, string MaliyetFiyat, string IskontoOran,string Kdv, string Bakiye)
        {
            try
            {
                KalemViewModel kalemItem = new KalemViewModel();
                var model = (List<KalemViewModel>)TempData["Kalemler"];

                var Depolar = MyBll.GetYaslandirma(StokKodu);

                var MaxMiktar = Depolar.First(t => t.Fiyat == Convert.ToDecimal(MaliyetFiyat.Replace(',','.')) && t.DepoKodu == Depo);

                var totalMiktar = model.Where(t => t.StokKodu == StokKodu && t.DepoKodu == Depo && t.MaliyetFiyat == Convert.ToDecimal(MaliyetFiyat.Replace(',', '.')) && t.Tip != null).Select(t => t.Miktar).Sum();

            

                if(totalMiktar + Convert.ToDecimal(Miktar) > MaxMiktar.Miktar)
                {
                    TempData["Kalemler"] = model;
                    return "Depoda yeterli ürün yok lütfen Satıcı Siparişi girin";
                }
                else
                {
                    FatType Type;
                    if (Tip == 0)
                    {
                        Type = FatType.Bloke;
                    }
                    else if (Tip == 2)
                    {
                        Type = FatType.OnFatura;
                    }
                    else
                    {
                        Type = FatType.Transit;
                    }

                    if(SiraNo != -1)
                    {
                        var UpdateItem = model.Where(t => t.Sira == SiraNo).First();
                        model.Remove(UpdateItem);

                        kalemItem.StokKodu = StokKodu;
                        kalemItem.StokIsim = StokIsim;
                        kalemItem.Sira = SiraNo;
                        kalemItem.Tip = Type;
                        kalemItem.DepoKodu = Depo;
                        kalemItem.DepoAdi = DepoAdi;
                        kalemItem.Miktar = Convert.ToDecimal(Miktar);//.Replace(',','.'));
                        kalemItem.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
                        kalemItem.MaliyetFiyat = Convert.ToDecimal(MaliyetFiyat);//.Replace(',', '.'));
                        kalemItem.ToplamTutar = kalemItem.Miktar * kalemItem.Fiyat;
                        kalemItem.ToplamMaliyetTutar = kalemItem.Miktar * kalemItem.MaliyetFiyat;
                    
                        kalemItem.IskontoOran = Convert.ToDecimal(IskontoOran);//.Replace(',', '.'));
                        kalemItem.IskontoluTutar = kalemItem.ToplamTutar - (kalemItem.IskontoOran / 100 * kalemItem.ToplamTutar);
                        kalemItem.ToplamKDVliTutar = kalemItem.IskontoluTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.IskontoluTutar);
                        kalemItem.Bakiye = Convert.ToDecimal(Bakiye);//.Replace(',', '.'));

                        model.Add(kalemItem);
                    }
                    else
                    {
                        var newItem = model.Where(t => t.StokKodu == StokKodu && t.DepoKodu == Depo && t.MaliyetFiyat == Convert.ToDecimal(MaliyetFiyat) && t.IskontoOran == Convert.ToDecimal(IskontoOran) && t.Tip == Type);

                        if (newItem.Count() > 0)
                        {
                            KalemViewModel modelFirst = newItem.First();

                            model.Remove(modelFirst);

                            modelFirst.Miktar += Convert.ToDecimal(Miktar);//.Replace(',','.'));
                            modelFirst.Fiyat = Convert.ToDecimal(Fiyat);//.Replace(',', '.'));
                            modelFirst.ToplamTutar = modelFirst.Miktar * modelFirst.Fiyat;
                            modelFirst.ToplamMaliyetTutar = modelFirst.Miktar * modelFirst.MaliyetFiyat;                       
                            modelFirst.IskontoluTutar = modelFirst.ToplamTutar - (modelFirst.IskontoOran / 100 * modelFirst.ToplamTutar);
                            kalemItem.ToplamKDVliTutar = kalemItem.IskontoluTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.IskontoluTutar);
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

                            if (Tip == 0)
                            {
                                kalemItem.Tip = FatType.Bloke;
                            }
                            else if (Tip == 2)
                            {
                                kalemItem.Tip = FatType.OnFatura;
                            }
                            else
                            {
                                kalemItem.Tip = FatType.Transit;
                            }

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
                            kalemItem.ToplamKDVliTutar = kalemItem.IskontoluTutar + (Convert.ToDecimal(Kdv) / 100 * kalemItem.IskontoluTutar);
                            kalemItem.Bakiye = Convert.ToDecimal(Bakiye);//.Replace(',', '.'));

                            model.Add(kalemItem);
                        }
                    }               
                }
                model.OrderBy(t => t.Sira);
                TempData["Kalemler"] = model;
                return "Kalem başarıyla eklendi";
            }
            catch (Exception e)
            {
                return e.Message;
            }
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
        public ActionResult GridViewPartialSiparisKalemDelete(int id)
        {
            
            var model = (List<KalemViewModel>)TempData["Kalemler"];
            KalemViewModel kalem = model.Where(t=> t.Sira == id).First();
            model.Remove(kalem);
            TempData["Kalemler"] = model;
            return PartialView("_GridViewPartialSiparisKalem", model);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialBlokeCoz()
        {
            List<string> SelectedRows = (List<string>)TempData["SelectedBlokeler"];

            List<BlokeCozViewModel> blokeler = new List<BlokeCozViewModel>();

            foreach (string s in SelectedRows)
            {
                blokeler.Add(MyBll.GetBlokeler().Where(t => t.ID == Convert.ToInt32(s)).First());               
            }

            int i = 0;
            foreach (BlokeCozViewModel bloke in blokeler)
            {
                blokeler[i].ID = i;
                i++;
            }
            TempData["SelectedBlokeler"] = SelectedRows;
            return PartialView("_GridViewPartialBlokeCoz", blokeler);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialBloke()
        {
            var model = MyBll.GetBlokeler();
            return PartialView("_GridViewPartialBloke", model);
        }

        [HttpGet]
        public decimal StokPaydasiGetir(string stokKodu)
        {
            decimal result = MyBll.StokPaydasiGetir(stokKodu);
            return result;
        }

        public ActionResult Print(string id)
        {
          
            Siparisler _siparisler = MyBll.GetSiparisler().First(t => t.FatNo == id);
            List<SiparisKalemler> _kalemler = MyBll.GetSiparisKalemler(id);

            SiparisPrint printModel = new SiparisPrint();
            printModel.Siparis = _siparisler;
            printModel.Kalemler = _kalemler;

            string fileName = "Siparis" + _siparisler.Cari_Isim.Replace('.', '_') + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;

            rptSiparis.DataSourceDemanded += (s, e) =>
            {
                ((Aruna_DTN_Entegrasyon.Reports.xptSiparisReport)s).DataSource =printModel;
                ((Aruna_DTN_Entegrasyon.Reports.xptSiparisReport)s).Name = fileName;
            };


            return View("Print", rptSiparis);

        }

       
    }
}