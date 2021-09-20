using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.ViewModels
{
    public class TeklifViewModel
    {
        public string FatNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Tarih { get; set; }
        public string CariKod { get; set; }
        public string CariIsim { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime GecerlilikTarihi { get; set; }
        public string Plasiyer { get; set; }
        public string PlasiyerAd { get; set; }
        public string SevkAdresi { get; set; }
        public string AciklamaSahasi { get; set; }
    }
}