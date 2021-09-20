using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class TeklifKalemler
    {
        public string FatNo { get; set; }
        public int Sira { get; set; }
        public string StokKodu { get; set; }
        public string StokAd { get; set; }
        public int DepoKodu { get; set; }
        public string DepoAdi { get; set; }
        public decimal Miktar { get; set; }
        public string Birim { get; set; }
        public decimal Fiyat { get; set; }
        public decimal MaliyetFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal ToplamMaliyetTutar { get; set; }
        public decimal IskontoOran { get; set; }
        public decimal IskontoluTutar { get; set; }
        public decimal Bakiye { get; set; }
        public decimal KdvOrani { get; set; }
    }
}