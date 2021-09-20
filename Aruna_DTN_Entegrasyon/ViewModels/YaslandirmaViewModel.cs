using Aruna_DTN_Entegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.ViewModels
{
    public class YaslandirmaViewModel
    {
        public string D_S { get; set; }
        public string StokKodu { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Miktar { get; set; }
        public decimal Fiyat { get; set; }
        public string DepoKodu { get; set; }
        public string Sira { get; set; }
    }
}