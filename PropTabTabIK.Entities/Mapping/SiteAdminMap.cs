using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Mapping
{
    public class SiteAdminMap : CoreMap<SiteAdmin>
    {
        public override void Configure(EntityTypeBuilder<SiteAdmin> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(true);

            builder.HasOne(x => x.SuperAdmin).WithMany(x => x.SiteAdmins).HasForeignKey(x => x.SuperAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.CompanyAdmins).WithOne(x => x.SiteAdmin).HasForeignKey(x => x.SiteAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);

        }


    }
}
