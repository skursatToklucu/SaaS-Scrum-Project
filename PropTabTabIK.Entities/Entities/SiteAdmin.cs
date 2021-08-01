using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Entities
{
    public class SiteAdmin : CoreEntity
    {


        public Guid SuperAdminID { get; set; }

        //Navigation Property
        public virtual SuperAdmin SuperAdmin { get; set; }

        public virtual ICollection<CompanyAdmin> CompanyAdmins { get; set; }

    }
}
