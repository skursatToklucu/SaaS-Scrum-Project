using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class PackageMap : SideMap<Package>
    {
        public override void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasMany(x => x.Companies).WithOne(x => x.Package).HasForeignKey(x => x.PackageID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            base.Configure(builder);
        }
    }
}
