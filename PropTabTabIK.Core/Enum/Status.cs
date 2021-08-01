using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Core.Enum
{
    public enum Status
    {
        [Display(Name ="Hesabını Aktif Etmedi")]
        None = 0,
        [Display(Name = "Hesabını Aktif Etti")]
        Active = 1,
        [Display(Name ="Hesabı Askıya Alındı")]
        Deactive = 2,
        [Display(Name ="Hesabi Silindi")]
        Deleted = 3,

    }
}
