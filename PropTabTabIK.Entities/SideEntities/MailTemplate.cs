using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    //Mail Sablonu
    public class MailTemplate : SideEntity
    {
        public Guid UserID { get; set; }

        [Display(Name = "Konu")]
        public string Subject { get; set; }


        [Display(Name = "İçerik")]
        public string Content { get; set; }

    }
}
