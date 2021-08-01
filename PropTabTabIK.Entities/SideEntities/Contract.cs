using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    public class Contract : SideEntity
    {
        public Guid OwnerID { get; set; }
        public Guid CompanyID { get; set; }

        public Guid PackageID { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Content { get; set; }

    }
}
