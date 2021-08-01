using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    public class Unit : SideEntity
    {
        //Departmana bagli birim veya birimler

        [Display(Name = "Çalışan Sayısı")]
        public int EmployeeCount { get; set; }

        public Guid DepartmentID { get; set; }

        //Navigation Property

        public virtual Department Department { get; set; }
    }
}
