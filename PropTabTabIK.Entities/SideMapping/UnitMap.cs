using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class UnitMap : SideMap<Unit>
    {
        public override void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasOne(x => x.Department).WithMany(x => x.Units).HasForeignKey(x => x.DepartmentID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
