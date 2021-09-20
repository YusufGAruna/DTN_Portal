using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class TeklifPrint
    {
        public string FatNo { get; set; }
        public DateTime Tarih { get; set; }
        public string Cari_Kod { get; set; }
        public string Cari_Isim { get; set; }
        public DateTime GecerlilikTarihi {get;set;}  
        public string Plasiyer { get; set; }
        public string Plasiyer_Ad { get; set; }
        public string Proje { get; set; }
        public decimal Brut_Tutar { get; set; }
        public decimal Isk_Tutar { get; set; }
        public decimal KDVSizToplam { get; set; }
        public decimal Kdv { get; set; }
        public decimal Genel_Toplam { get; set; }
        public string SevkAdresi { get; set; }
        public string AciklamaSahasi { get; set; }
      
        public List<TeklifPrintKalem> TeklifKalemleri { get; set; }
    }
    public class TeklifPrintKalem
    {
        public int Sira { get; set; }
        public string StokKodu { get; set; }
        public string StokAd { get; set; }
        public int DepoKodu { get; set; }
        public string DepoAdi { get; set; }
        public decimal Miktar { get; set; }
        public string Birim { get; set; }
        public decimal Fiyat { get; set; }
        public decimal IskontoOran { get; set; }
        public decimal IskontoluFiyat { get; set; }
        public decimal IskontoluTutar { get; set; }
        public decimal KdvTutari { get; set; }
        public decimal ToplamTutar { get; set; }
      
    }
}