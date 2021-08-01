using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Core.Mapping
{
    public class SideMap<T> : IEntityTypeConfiguration<T> where T : SideEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).HasColumnName("ID");
            builder.Property(x => x.Status).IsRequired(true);
        }
    }
}
