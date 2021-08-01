using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class DepartmentMap : SideMap<Department>
    {
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasOne(x => x.CompanyAdmin).WithMany(x => x.Departments).HasForeignKey(x => x.CompanyID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasOne(x => x.Location).WithMany(x => x.Departments).HasForeignKey(x => x.LocationID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.Units).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
