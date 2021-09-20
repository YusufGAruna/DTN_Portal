using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class SaticiSiparisKalemler
    {
        public int Sira { get; set; }
        public string FatNo { get; set; }
        public string Cari_Kod { get; set; }
        public string Cari_Isim { get; set; }
        public DateTime Tarih { get; set; }
        public string StokKodu { get; set; }
        public string StokAd { get; set; }
        public decimal Miktar { get; set; }
        public decimal NetFiyat { get; set; }
        public decimal BrutFiyat { get; set; }
        public decimal Iskonto { get; set; }
        public decimal Kdv { get; set; }
        public decimal Tutar { get; set; }
        public int HareketDurumu { get; set; }
    }
}