using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideEntities
{
    public class Location : SideEntity
    { 
        // Navigation Property

        public virtual ICollection<Department> Departments { get; set; }
    }
}
