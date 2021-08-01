using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class LocationMap : SideMap<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {

            builder.HasMany(x => x.Departments).WithOne(x => x.Location).HasForeignKey(x => x.LocationID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
