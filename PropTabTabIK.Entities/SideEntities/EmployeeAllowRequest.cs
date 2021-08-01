using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    public class EmployeeAllowRequest
    {
        public Guid EmployeeID { get; set; }

        public Guid AllowRequestID { get; set; }

        public byte AllowRight { get; set; }
    }
}
