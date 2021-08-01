using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Core.Enum
{
    //Bekleyen izinler, abanslar vs durumlarini belirtmek icin kullanilacak
    public enum State
    {
        [Display(Name = "Beklemede")]
        Pending = 0, // Beklemede

        [Display(Name = "Onaylandı")]
        Confirmed = 1, //Onaylandı

        [Display(Name = "Onaylanmadı")]
        Denied = 2  //Onaylanmadı
    }
}
