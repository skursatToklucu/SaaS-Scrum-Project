using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class AllowRequestMap : SideMap<AllowRequest>
    {
        public override void Configure(EntityTypeBuilder<AllowRequest> builder)
        {
            builder.HasOne(x => x.Employee).WithMany(x => x.AllowRequests).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
