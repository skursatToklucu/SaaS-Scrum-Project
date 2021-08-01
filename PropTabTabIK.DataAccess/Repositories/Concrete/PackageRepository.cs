using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class PackageRepository : SideRepository<Package>
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;
        public PackageRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _httpContext = httpContext;
            _context = context;

        }

        /// <summary>
        /// Company'e ait paket var mi diye kontrol eder.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AnyPackage(Guid id)
        {
            return _context.CompanyAdmins.Select(x => x.PackageID).Contains(id);
        }

        /// <summary>
        /// Paketin Statusunu deactive yapar.
        /// </summary>
        /// <param name="id"></param>
        public void DeletePackage(Guid id)
        {
            DeActivate(id);
        }
        /// <summary>
        /// Paketin resmini getirir.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public string GetPhoto(Package package)
        {
            return _context.Packages.Where(X => X.ID == package.ID).Select(x => x.Photo).FirstOrDefault();

        }


    }
}
