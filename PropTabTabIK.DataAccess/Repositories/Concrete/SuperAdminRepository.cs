using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class SuperAdminRepository : Repository<SuperAdmin>
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;
        public SuperAdminRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }


    }
}
