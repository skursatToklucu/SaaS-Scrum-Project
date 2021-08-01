using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //Paketler
    public class Package : SideEntity
    {
        [Display(Name = "Ücreti")]
        public decimal Price { get; set; }

        [Display(Name = "Çalışan Sayısı")]
        public int EmployeeCount { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public int ExpireTime { get; set; }

        [Display(Name = "Paket Kapak Fotoğrafı")]
        public string Photo { get; set; }

        //Navigation Property

        public virtual ICollection<CompanyAdmin> Companies { get; set; }

    }
}
