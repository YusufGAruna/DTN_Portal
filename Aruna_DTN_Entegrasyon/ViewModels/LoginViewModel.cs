using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aruna_DTN_Entegrasyon.ViewModels
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string MagazaKodu { get; set; }
    }
}