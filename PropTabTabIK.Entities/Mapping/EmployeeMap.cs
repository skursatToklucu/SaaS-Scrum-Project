using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.Mapping
{
    public class EmployeeMap : CoreMap<Employee>
    {

        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasOne(x => x.CompanyAdmin).WithMany(x => x.Employees).HasForeignKey(x => x.CompanyAdminID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.AdvancePayments).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.AllowRequests).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.Expenses).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasMany(x => x.OvertimePeriods).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            base.Configure(builder);
        }

    }
}
