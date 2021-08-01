using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class SiteAdminRepository : Repository<SiteAdmin>
    {
        private readonly ProjectContext _context;
        private CompanyAdminRepository _companyAdminRepository;
        private IHttpContextAccessor _httpContext;
        public SiteAdminRepository(ProjectContext context, IHttpContextAccessor httpContext, CompanyAdminRepository companyAdminRepository) : base(context, httpContext)
        {
            _context = context;
            _httpContext = httpContext;
            _companyAdminRepository = companyAdminRepository;
        }

        /// <summary>
        /// Sirketleri eger baska bir site admin silinirse devreder.
        /// </summary>
        /// <param name="siteAdmin"></param>
        public void CompaniesTransferToAnotherSiteAdmin(SiteAdmin siteAdmin)
        {
            List<CompanyAdmin> companiesOfSiteAdmin = _context.CompanyAdmins.Where(x => x.SiteAdminID == siteAdmin.ID).ToList();

            List<SiteAdmin> siteAdmins = GetActive();
            foreach (var item in companiesOfSiteAdmin)
            {
                item.SiteAdminID = siteAdmins.AsQueryable().Select(x => x.ID).FirstOrDefault();
            }
            _companyAdminRepository.UpdateRange(companiesOfSiteAdmin);
        }

        /// <summary>
        /// SiteAdmin icin temel bilgileri ayarlar.
        /// <ul>
        /// <li>Profil Resmi</li>
        /// <li>,Status</li>
        /// <li>,Role</li>
        /// <li>,Sifre</li>
        /// </ul>
        /// </summary>
        /// <param name="siteAdmin"></param>
        /// <param name="imagePP"></param>
        /// <returns></returns>
        public SiteAdmin CreateBasic(SiteAdmin siteAdmin, IFormFile imagePP)
        {
            SiteAdmin SiteAdmin = NullOrImage(siteAdmin, imagePP);

            SiteAdmin.Status = Core.Enum.Status.None;
            SiteAdmin.Role = Core.Enum.Role.SiteAdministator;
            SiteAdmin.Password = GenerateToken(8);

            return SiteAdmin;

        }


    }
}
