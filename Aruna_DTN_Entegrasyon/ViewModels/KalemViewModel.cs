using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.ViewModels
{
    public class KalemViewModel
    {
        public int Sira { get; set; }
        public string StokKodu { get; set; }
        public string StokIsim { get; set; }
        public string DepoKodu { get; set; }
        public string DepoAdi { get; set; }
        public FatType? Tip { get; set; }
        public decimal Miktar { get; set; }
        public decimal Fiyat { get; set; }
        public decimal MaliyetFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal Kdv { get; set; }
        public decimal ToplamKDVliTutar { get; set; }
        public decimal ToplamMaliyetTutar { get; set; }
        [DefaultValue(0)]
        public decimal IskontoOran { get; set; }
        public decimal IskontoluTutar { get; set; }
        public decimal Bakiye { get; set; }


    }
}