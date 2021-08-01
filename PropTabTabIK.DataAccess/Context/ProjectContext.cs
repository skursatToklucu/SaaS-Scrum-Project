using Microsoft.EntityFrameworkCore;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.Mapping;
using PropTabTabIK.Entities.SideEntities;
using PropTabTabIK.Entities.SideMapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.DataAccess.Context
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {


        }

        //Main Entities

        #region DbSet Main Entities
        public DbSet<SuperAdmin> SuperAdmins { get; set; }

        public DbSet<SiteAdmin> SiteAdmins { get; set; }

        public DbSet<CompanyAdmin> CompanyAdmins { get; set; }

        public DbSet<Employee> Employees { get; set; }
        #endregion

        //Side Entities

        #region DbSet Side Entities
        public DbSet<AdvancePayment> AdvancePayments { get; set; }

        public DbSet<AllowRequest> AllowRequests { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<MailTemplate> MailTemplates { get; set; }

        public DbSet<OvertimePeriod> OvertimePeriods { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<EmployeeAllowRequest> EmployeeAllowRequests { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Apply Configuration for Main Entities
            modelBuilder.ApplyConfiguration(new SuperAdminMap());
            modelBuilder.ApplyConfiguration(new SiteAdminMap());
            modelBuilder.ApplyConfiguration(new CompanyAdminMap());
            modelBuilder.ApplyConfiguration(new EmployeeMap());
            #endregion

            #region Apply Configuration for Side Entities
            modelBuilder.ApplyConfiguration(new AdvancePaymentMap());
            modelBuilder.ApplyConfiguration(new AllowRequestMap());
            modelBuilder.ApplyConfiguration(new DepartmentMap());
            modelBuilder.ApplyConfiguration(new ExpenseMap());
            modelBuilder.ApplyConfiguration(new LocationMap());
            modelBuilder.ApplyConfiguration(new OvertimePeriodMap());
            modelBuilder.ApplyConfiguration(new PackageMap());
            modelBuilder.ApplyConfiguration(new UnitMap());
            #endregion


            modelBuilder.Entity<EmployeeAllowRequest>().HasKey(x => new { x.EmployeeID, x.AllowRequestID });

            base.OnModelCreating(modelBuilder);
        }


    }

}
