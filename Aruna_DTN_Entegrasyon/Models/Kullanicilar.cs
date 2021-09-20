using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    [Table("Kullanicilar")]
    public class Kullanicilar
    {
        [Key]
        public string UserName { get; set; }
        public string KullaniciAdi { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [EnumDataType(typeof(RolType))]
        public RolType Rol { get; set; }
        public int MagazaKodu { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal IskontoOran { get; set; }
    }

    public enum RolType
    {
        Admin,
        Siparis,
        Rapor
    }
}