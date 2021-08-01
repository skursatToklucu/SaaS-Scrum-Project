using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Core.Enum;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //Fazla Mesai
    public class OvertimePeriod : SideEntity
    {
        [Display(Name = "Açıklama")]
        public string Description { get; set; }


        private DateTime? startDate = null;
        [Display(Name = "Fazla Mesai Başlangıç Tarihi ")]
        public DateTime StartDate
        {
            get
            {
                return this.startDate.HasValue ? this.startDate.Value : DateTime.Now;
            }

            set { this.startDate = value; }
        }

        [Display(Name = "Fazla Mesai Bitiş Tarihi ")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Onaylanma Tarihi")]
        public DateTime ConfirmedDate { get; set; }
        public bool IsConfirmed { get; set; }

        public State State { get; set; }

        public Guid EmployeeID { get; set; }
        
        //Navigation Property

        public virtual Employee Employee { get; set; }
    }
}
