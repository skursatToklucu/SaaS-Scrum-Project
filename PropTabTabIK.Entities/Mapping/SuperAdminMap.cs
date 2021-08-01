using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Mapping
{
    public class SuperAdminMap : CoreMap<SuperAdmin>
    {

        public override void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {

            builder.HasMany(x => x.SiteAdmins).WithOne(x => x.SuperAdmin).HasForeignKey(x => x.SuperAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
