using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.Entities
{
    public class Employee : CoreEntity
    {
        [Required(ErrorMessage = "İşe başlama tarihi olmak zorundadır.")]
        [Display(Name = "İşe Başlama Tarihi")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "İşten Ayrılma Tarihi")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }


        [Display(Name = "Doğum Tarihi")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1950", "1/1/2004", ErrorMessage = "Girdiğiniz Tarih 1950 ila 2004 arasında olmalı")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "T.C Kimlik Numarası")]
        public long? TCNo { get; set; }

        public double Salary { get; set; }


        public ulong? IBAN { get; set; }

        [Display(Name = "Pozisyon")]
        public string Position { get; set; }

        public byte AllowCount { get; set; } = 14;

        private double advancePaymentRight;
        public double AdvancePaymentRight
        {
            get
            {
                return advancePaymentRight = Salary * 0.40;
            }
            set
            {
                advancePaymentRight = value;
            }
        }

        public Guid CompanyAdminID { get; set; }


        //Navigation Property

        public virtual CompanyAdmin CompanyAdmin { get; set; }

        public virtual ICollection<AdvancePayment> AdvancePayments { get; set; }

        public virtual ICollection<AllowRequest> AllowRequests { get; set; }

        public virtual ICollection<OvertimePeriod> OvertimePeriods { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }




    }
}
