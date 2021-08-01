using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    // Sirketin bolumleri
    public class Department : SideEntity
    {
        [Display(Name = "Çalışan Sayısı")]
        public int EmployeeCount { get; set; }

        public Guid LocationID { get; set; }

        public Guid CompanyID { get; set; }

        //Navigation Property

        public virtual CompanyAdmin CompanyAdmin { get; set; }

        public virtual Location Location { get; set; }

        public virtual ICollection<Unit> Units { get; set; }

    }
}
