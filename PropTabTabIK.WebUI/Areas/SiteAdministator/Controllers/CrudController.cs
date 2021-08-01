using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.SideEntities;
using PropTabTabIK.WebUI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.SiteAdministator.Controllers
{
    [Area("SiteAdministator")]
    [Route("[area]/[controller]/[action]")]
    public class CrudController : Controller
    {

        SuperAdminRepository _superAdminRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;
        EmployeeRepository _employeeRepository;
        PackageRepository _packageRepository;
        MailTemplateRepository _mailTemplateRepository;
        ContractRepository _contractRepository;
        public CrudController(ProjectContext context, IHttpContextAccessor httpContext)
        {

            _superAdminRepository = new SuperAdminRepository(context, httpContext);
            _contractRepository = new ContractRepository(context, httpContext);
            _packageRepository = new PackageRepository(context, httpContext);
            _mailTemplateRepository = new MailTemplateRepository(context, httpContext);
            _employeeRepository = new EmployeeRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
        }

        public IActionResult Index()
        {
            #region SiteAdmin ViewBags

            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");

            #endregion
            return View(_companyAdminRepository.GetDefault(x => x.SiteAdminID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))));
        }
        public IActionResult CreateCompanyAdmin()
        {
            #region ViewBag
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            Guid getSiteAdminID = Guid.Parse(HttpContext.Session.GetString("SiteAdminID"));
            ViewBag.Packages = _packageRepository.GetAll();
            #endregion
            return View();
        }
        [HttpPost]
        public IActionResult CreateCompanyAdmin(CompanyAdmin companyAdmin, IFormFile logo, IFormFile imagePP)
        {
            #region ViewBag
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            ViewBag.Packages = _packageRepository.GetAll();

            #endregion

            if (_companyAdminRepository.Any(x => x.Email == companyAdmin.Email) || _employeeRepository.Any(x => x.Email == companyAdmin.Email) || _siteAdminRepository.Any(x => x.Email == companyAdmin.Email))
            {
                TempData["Msg"] = "Bu mail zaten kayıtlı";
                return View(companyAdmin);
            }
            if (!(_companyAdminRepository.IsImage(imagePP)) || !(_companyAdminRepository.IsImage(logo)))
            {
                TempData["IsImage"] = "Lütfen resim yükleyiniz";
                return View();
            }
            if (_companyAdminRepository.IsFileSizeBig(imagePP) || _companyAdminRepository.IsFileSizeBig(logo))
            {
                TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                return View();
            }
            ViewBag.Success = "Başarıyla eklendi...";


            _companyAdminRepository.Add(_companyAdminRepository.CreateBasic(companyAdmin, imagePP, logo));
            _contractRepository.ContractMail(_contractRepository.CreateContract(Guid.Parse(HttpContext.Session.GetString("SiteAdminID")), companyAdmin.ID, companyAdmin.PackageID));


            return View();
        }




        public IActionResult Delete(Guid id)
        {

            _companyAdminRepository.Remove(_companyAdminRepository.GetByID(id));
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Update(Guid id)
        {
            #region SiteAdmin  ViewBag's
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion
            ViewBag.Packages = _packageRepository.GetAll();
            CompanyAdmin companyAdmin = _companyAdminRepository.GetByID(id);
            return View(companyAdmin);
        }
        [HttpPost]
        public IActionResult Update(CompanyAdmin companyAdmin, IFormFile logo, IFormFile imagePP)
        {
            #region SiteAdmin ViewBag'S
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion

            ViewBag.Packages = _packageRepository.GetAll();

            if (!(_companyAdminRepository.IsImage(imagePP)))
            {
                TempData["IsImage"] = "Lütfen resim yükleyiniz";
                return View(companyAdmin);
            }
            if (_companyAdminRepository.IsFileSizeBig(imagePP))
            {
                TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                return View(companyAdmin);
            }


            CompanyAdmin companyAdminUpdate = _companyAdminRepository.GetCompanyLogo(_companyAdminRepository.GetProfilPicture(companyAdmin, imagePP), logo);



            CompanyAdmin companyAdmin1 = _companyAdminRepository.GetByDefault(x => x.ID == companyAdmin.ID);

            if (companyAdminUpdate.Email != companyAdmin1.Email)
            {
                if (_companyAdminRepository.Any(x => x.Email == companyAdminUpdate.Email) || _employeeRepository.Any(x => x.Email == companyAdminUpdate.Email) || _siteAdminRepository.Any(x => x.Email == companyAdminUpdate.Email) || companyAdminUpdate.Email == companyAdmin1.Email)
                {
                    TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                    return View(companyAdminUpdate);
                }
                else
                    _companyAdminRepository.SendNewMailAddress(companyAdminUpdate);
            }
            _companyAdminRepository.Update(companyAdminUpdate);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult ChangeStatus(Guid id)
        {
            if (_companyAdminRepository.GetByDefault(x => x.ID == id).Status == Core.Enum.Status.Active)
                _companyAdminRepository.DeActivate(id);
            else
                _companyAdminRepository.Activate(id);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateMailTemplate()
        {
            #region ViewBag
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            ViewBag.Packages = _packageRepository.GetAll();

            #endregion
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            return View();

        }

        [HttpPost]
        public IActionResult CreateMailTemplate(MailTemplate mailTemplate)
        {
            mailTemplate.Status = Core.Enum.Status.Active;
            _mailTemplateRepository.Add(mailTemplate);

            return RedirectToAction("Index", "Home");

        }

        public IActionResult DeleteMail(Guid id)
        {
            _mailTemplateRepository.Remove(_mailTemplateRepository.GetByID(id));
            return RedirectToAction("Index", "Home");
        }


        public IActionResult UpdateMail(Guid id)
        {
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            return View(_mailTemplateRepository.GetByID(id));

        }
        [HttpPost]
        public IActionResult UpdateMail(MailTemplate mailTemplate)
        {

            _mailTemplateRepository.Update(mailTemplate);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult SendingMail(Guid id)
        {
            #region SiteAdmin  ViewBag's
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion

            ViewBag.Companies = _companyAdminRepository.GetDefault(x => x.SiteAdminID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID")));

            return View(_mailTemplateRepository.GetByID(id));

        }

        [HttpPost]
        public IActionResult SendingMail(MailTemplate mailTemplate, Guid mailAddress)
        {
            #region SiteAdmin  ViewBag's
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion

            ViewBag.Companies = _companyAdminRepository.GetDefault(x => x.SiteAdminID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID")));

            _companyAdminRepository.SendMail(_companyAdminRepository.GetByID(mailAddress), mailTemplate.ID);
            TempData["success"] = "Mailiniz basarılı bir sekilde gönderildi";


            return View();

        }

    }
}


