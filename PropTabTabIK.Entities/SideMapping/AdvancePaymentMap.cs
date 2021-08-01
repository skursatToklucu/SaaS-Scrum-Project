using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropTabTabIK.Core.Mapping;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.Entities.SideMapping
{
    public class AdvancePaymentMap : SideMap<AdvancePayment>
    {
        public override void Configure(EntityTypeBuilder<AdvancePayment> builder)
        {

            builder.HasOne(x => x.Employee).WithMany(x => x.AdvancePayments).HasForeignKey(x => x.EmployeeID).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);


            base.Configure(builder);
        }
    }
}
