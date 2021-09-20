using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.ViewModels
{
    public class BlokeCozViewModel
    {
        public int ID { get; set; }
        public int SubeKodu { get; set; }
        public string SiparisNo { get; set; }
        public int Sira { get; set; }
        public string CariKodu { get; set; }
        public string CariAdi { get; set; }
        public string StokKodu { get; set; }
        public string StokAdi { get; set; }
        public int DepoKodu { get; set; }
        public decimal Miktar { get; set; }
        public decimal BlokeliMiktar { get; set; }
    }
}