using Aruna_DTN_Entegrasyon.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class Teklifler
    {
        [Key]
        public string FatNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Tarih { get; set; } = DateTime.Now.Date;
        public string Cari_Kod { get; set; }
        public string Cari_Isim { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime GecerlilikTarihi
        {
            get;
            set;
        } = DateTime.Now.AddDays(7).Date;
        public string Plasiyer { get; set; }
        public string Plasiyer_Ad { get; set; }
        public string Proje { get; set; }
        public decimal Brut_Tutar { get; set; }
        public decimal Isk_Tutar { get; set; }
        public decimal Kdv { get; set; }
        public decimal Genel_Toplam { get; set; }
        public string AciklamaSahasi { get; set; }

        public Cariler Cari { get; set; }
        public List<KalemViewModel> FaturaKalemler { get; set; }
    }
}