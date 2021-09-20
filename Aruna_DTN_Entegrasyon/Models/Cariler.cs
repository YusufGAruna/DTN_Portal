using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.Models
{
    public partial class Cariler
    {
        [Key]
        public string CariKod { get; set; }
        public string CariIsim { get; set; }
        public string Adres { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string TcNo { get; set; }
        public string EMail { get; set; }
        public string IlgiliKisi { get; set; }
        public string TeslimatAdresi { get; set; }
    }
}