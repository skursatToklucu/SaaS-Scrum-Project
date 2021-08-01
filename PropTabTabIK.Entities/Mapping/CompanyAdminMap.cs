using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Mapping
{
    public class CompanyAdminMap : CoreMap<CompanyAdmin>
    {
        public override void Configure(EntityTypeBuilder<CompanyAdmin> builder)
        {

            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.Surname).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.Email).IsRequired(true);
            builder.Property(x => x.Password).IsRequired(true);

            builder.HasOne(x => x.SiteAdmin).WithMany(x => x.CompanyAdmins).HasForeignKey(x => x.SiteAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.Employees).WithOne(x => x.CompanyAdmin).HasForeignKey(x => x.CompanyAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.Departments).WithOne(x => x.CompanyAdmin).HasForeignKey(x => x.CompanyID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasOne(x => x.Package).WithMany(x => x.Companies).HasForeignKey(x => x.PackageID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
