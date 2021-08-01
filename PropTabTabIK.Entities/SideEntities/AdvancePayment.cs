using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Core.Enum;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //Avans Talepleri 
    public class AdvancePayment : SideEntity
    {
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Avans Miktarı")]
        public double Amount { get; set; }

        [Display(Name = "Talep Tarihi")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Onaylanma Tarihi")]
        public DateTime ConfirmedDate { get; set; }

        public AdvancePaymentType AdvancePaymentType { get; set; }


        public State State { get; set; }

        public string CompanyDescription { get; set; }

        public Guid EmployeeID { get; set; }

        //Navigation Property

        public virtual Employee Employee { get; set; }

    }
}
