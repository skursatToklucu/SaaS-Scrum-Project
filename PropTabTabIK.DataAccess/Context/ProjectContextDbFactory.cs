using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.DataAccess.Context
{
    public class ProjectContextDbFactory : IDesignTimeDbContextFactory<ProjectContext>
    {
        public ProjectContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-B77ITTF;Database=ScrumDB;Trusted_Connection=true");

            //Emre
            //var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            //optionsBuilder.UseSqlServer(@"server =.; DataBase = ProbTabTabIK; uid = sa; Password = 123; ");

            ////Fatma
            //var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-19NARKN;Database=ScrumDB;Trusted_Connection=true");

            return new ProjectContext(optionsBuilder.Options);
        }
    }
}
