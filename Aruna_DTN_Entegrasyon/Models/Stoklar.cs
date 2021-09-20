using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class Stoklar
    {
        public string StokKodu { get; set; }
        public string StokAdi { get; set; }
        public string GrupKodu { get; set; }
        public decimal Fiyat { get; set; }
        public decimal KutuKati { get; set; }
        public decimal ToplamBakiye { get; set; }
        public decimal DepoToplamBakiye { get; set; }
        public decimal SaticiSiparisMiktar { get; set; }
        public decimal TransitBakiye { get; set; }
        public decimal Kdv { get; set; }
    }
}