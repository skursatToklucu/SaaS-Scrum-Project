using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.Entities
{
    public class CompanyAdmin : CoreEntity
    {

        [Display(Name = "Şirket İsmi")]
        public string CompanyName { get; set; }

        [Display(Name = "Şirket Logosu")]
        public string CompanyLogo { get; set; }

        public Guid SiteAdminID { get; set; }

        //Encapsulation PuurchasePackageDate

        private DateTime? startDate = null;
        [Required(ErrorMessage = "Paket başlatma tarihi olmak zorundadır.")]
        [Display(Name = "Paketin Başlama Tarihi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{DD/MM/YYYY}")]
        [DataType(DataType.Date)]
        public DateTime PurchasePackageDate
        {
            get
            {
                return this.startDate.HasValue ? this.startDate.Value : DateTime.Now;
            }

            set { this.startDate = value; }
        }

        public Guid PackageID { get; set; }

        //Navigation Propperty

        public virtual SiteAdmin SiteAdmin { get; set; }

        public virtual Package Package { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Department> Departments { get; set; }



    }
}
