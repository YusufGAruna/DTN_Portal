using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    [Table("Siparisler")]
    public class Siparisler
    {
        public SipTip? SipTip { get; set; }
        [Key]
        public string FatNo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Tarih { get; set; } = DateTime.Now.Date;
        public string Cari_Kod { get; set; }
        public string Cari_Isim { get; set; }
        public int OdemeGun { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime OdemeTarih { get; set; }
        public string Plasiyer { get; set; }
        public string Plasiyer_Ad { get; set; }
        public string Proje { get; set; }
        public decimal Brut_Tutar { get; set; }
        public decimal Isk_Tutar { get; set; }
        public decimal Kdv { get; set; }
        public decimal Genel_Toplam { get; set; }     
        public string AciklamaSahasi { get; set; }

        public string TeklifNo { get; set; }
        public Cariler Cari { get; set; }

        public List<SiparisKalemler> FaturaKalemler { get; set; }

    }

    public enum SipTip
    {
        Parekende,
        TaliPromosyon,
        TaliToplu,
        TaliCariAlım,
        Toplu
    }
}