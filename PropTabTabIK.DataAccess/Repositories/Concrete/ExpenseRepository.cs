using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class ExpenseRepository : SideRepository<Expense>
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;
        public ExpenseRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _httpContext = httpContext;
            _context = context;

        }

    }
}
