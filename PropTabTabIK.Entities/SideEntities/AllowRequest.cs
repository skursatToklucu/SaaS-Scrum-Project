using Microsoft.AspNetCore.Mvc;
using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.Core.Enum;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //izin talepleri
    public class AllowRequest : SideEntity
    {
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [Display(Name = "Talep Tarihi")]
        public DateTime IssueDate { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [Display(Name = "Onaylanma Tarihi")]
        public DateTime ConfirmedDate { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [Display(Name = "İzin Başlangıç Tarihi ")]
        public DateTime StartDate { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [Display(Name = "İzin Bitiş Tarihi ")]
        public DateTime EndDate { get; set; }

        public int AllowTime { get; set; }

        public int RemainingAllowTime { get; set; }

        public State State { get; set; }

        public AllowType AllowType { get; set; }

        private byte totalAllowTime;
        public byte TotalAllowTime
        {
            get
            {
                switch (AllowType)
                {
                    case AllowType.annual:
                        return totalAllowTime = 14;
                    case AllowType.bereavement:
                        return totalAllowTime = 3;
                    case AllowType.maternity:
                        return totalAllowTime = 112;
                    case AllowType.disease:
                        return totalAllowTime = 40;
                    case AllowType.military:
                        return totalAllowTime = 21;
                    default:
                        return totalAllowTime = 0;

                }

            }
            set
            {
                totalAllowTime = value;
            }
        }

        public string CompanyDescription { get; set; }



        public Guid EmployeeID { get; set; }

        //NavigationProperty

        public virtual Employee Employee { get; set; }

    }
}
