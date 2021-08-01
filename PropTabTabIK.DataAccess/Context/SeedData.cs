using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropTabTabIK.DataAccess.Context
{
    public class SeedData
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var servicesScope = app.ApplicationServices.CreateScope();
            ProjectContext context = servicesScope.ServiceProvider.GetService<ProjectContext>();

            context.Database.Migrate();

            if (!context.SuperAdmins.Any())
            {
                context.SuperAdmins.AddRange(
                    new SuperAdmin { Role = Core.Enum.Role.SuperAdministator, Email = "admin@proptabtabhr.com", Status = Core.Enum.Status.Active, Password = "admin123" }
                    );
            }
            if (!context.MailTemplates.Any())
            {
                context.MailTemplates.AddRange(
                    new Entities.SideEntities.MailTemplate { Name = "Hoşgeldiniz Maili", Content = "Hoşgeldiniz...  " }
                    );
            }
            context.SaveChanges();
        }
    }
}
