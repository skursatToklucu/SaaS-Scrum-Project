using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Core.Enum;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //Masraf
    public class Expense : SideEntity
    {

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Dosya")]
        public string File { get; set; }

        public State State { get; set; }
        
        public bool IsConfirmed { get; set; }

        public Guid EmployeeID { get; set; }

        //Navigation Property

        public virtual Employee Employee { get; set; }

    }
}
