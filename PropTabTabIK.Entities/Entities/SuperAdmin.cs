using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Entities
{
    public class SuperAdmin : CoreEntity
    {

        //Navigation Property
        public virtual ICollection<SiteAdmin> SiteAdmins { get; set; }


    }
}
