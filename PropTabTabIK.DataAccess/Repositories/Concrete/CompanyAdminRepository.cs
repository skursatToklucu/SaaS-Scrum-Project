using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class CompanyAdminRepository : Repository<CompanyAdmin>
    {
        private readonly ProjectContext _context;
        private PackageRepository _packageRepository;
        private IHttpContextAccessor _httpContext;
        public CompanyAdminRepository(ProjectContext context, IHttpContextAccessor httpContext, PackageRepository packageRepository) : base(context, httpContext)
        {
            _packageRepository = packageRepository;
            _context = context;
            _httpContext = httpContext;

        }

        /// <summary>
        /// Logo varsa yukler yoksa default u alir
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        public CompanyAdmin NullOrLogo(CompanyAdmin entity, IFormFile logo)
        {
            if (logo == null)
            {
                entity.CompanyLogo = "https://image.shutterstock.com/image-vector/king-logo-stamp-empty-frame-600w-1366272653.jpg";
            }
            else
            {

                entity.CompanyLogo = LogoUpload(logo);

            }

            return entity;
        }



        /// <summary>
        /// CompanyAdmin icin temel bilgileri atar.
        /// </summary>
        /// <param name="companyAdmin"></param>
        /// <param name="imagePP"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        public CompanyAdmin CreateBasic(CompanyAdmin companyAdmin, IFormFile imagePP, IFormFile logo)
        {
            CompanyAdmin newCompanyAdmin = NullOrImage(companyAdmin, imagePP);
            CompanyAdmin newCompanyAdmin2 = NullOrLogo(newCompanyAdmin, logo);

           
            newCompanyAdmin2.Status = Core.Enum.Status.None;
            newCompanyAdmin2.Role = Core.Enum.Role.CompanyAdministator;
            newCompanyAdmin2.Password = GenerateToken(8);


            return newCompanyAdmin2;

        }

        /// <summary>
        /// Company Logosunu getirir
        /// </summary>
        /// <param name="companyAdmin"></param>
        /// <returns></returns>
        public string GetLogo(CompanyAdmin companyAdmin)

        {
            return _context.Set<CompanyAdmin>().Where(x => x.ID == companyAdmin.ID).Select(x => x.CompanyLogo).FirstOrDefault();
        }

        /// <summary>
        /// null gecilirse eski logoyu tutar
        /// </summary>
        /// <param name="companyAdmin"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        public CompanyAdmin GetCompanyLogo(CompanyAdmin companyAdmin, IFormFile logo)
        {
            if (logo == null) companyAdmin.CompanyLogo = GetLogo(companyAdmin);
            else companyAdmin.CompanyLogo = LogoUpload(logo);
            return companyAdmin;
        }


        public List<CompanyAdmin> UpcomingPackages(List<CompanyAdmin> companyAdmins)
        {

            List<CompanyAdmin> upcomingPackages = new List<CompanyAdmin>();

            List<Package> packages = _packageRepository.GetDefault(x => x.ID == companyAdmins.AsQueryable().Select(x => x.PackageID).FirstOrDefault());

            foreach (var item in packages)
            {
                int year = item.ExpireTime / 12;

                DateTime date = companyAdmins.AsQueryable().Select(x => x.PurchasePackageDate).FirstOrDefault();
                int ExpireYear = date.Year + year;

                CompanyAdmin companyAdmin = companyAdmins.AsQueryable().Where(x => x.PurchasePackageDate.Year - ExpireYear == 0).FirstOrDefault();


                upcomingPackages.Add(companyAdmin);
            }

            return upcomingPackages;
        }

    }
}
