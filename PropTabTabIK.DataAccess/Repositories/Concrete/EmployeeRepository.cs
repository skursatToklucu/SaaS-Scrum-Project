using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class EmployeeRepository : Repository<Employee>
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;
        public EmployeeRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }


        public Employee CreateBasic(Employee employee, IFormFile imagePP)
        {
            Employee Employee = NullOrImage(employee, imagePP);

            Employee.Status = Core.Enum.Status.None;
            Employee.Role = Core.Enum.Role.EmployeeArea;
            Employee.Password = GenerateToken(8);

            return Employee;

        }


    }
}
