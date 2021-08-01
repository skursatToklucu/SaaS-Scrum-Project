using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.SuperAdministator.Controllers
{
    [Area("SuperAdministator")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {
        private SuperAdminRepository _superAdminRepository;
        private SiteAdminRepository _siteAdminRepository;
        private CompanyAdminRepository _companyAdminRepository;
        private EmployeeRepository _employeeRepository;
        private PackageRepository _packageRepository;

        public HomeController(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _superAdminRepository = new SuperAdminRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _employeeRepository = new EmployeeRepository(context, httpContext);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
        }
        public IActionResult Index()
        {
            ViewBag.SuperAdminID = HttpContext.Session.GetString("SuperAdminID");

            return View(_siteAdminRepository.GetActive());
        }

        public IActionResult Create()
        {
            ViewBag.SuperAdminID = HttpContext.Session.GetString("SuperAdminID");
            return View();
        }

        [HttpPost]
        public IActionResult Create(SiteAdmin siteAdmin, IFormFile imagePP)
        {
            ViewBag.SuperAdminID = HttpContext.Session.GetString("SuperAdminID");
            if (!(_superAdminRepository.IsImage(imagePP)))
            {
                TempData["IsImage"] = "Lütfen resim yükleyiniz";
                return View();
            }
            if (_superAdminRepository.IsFileSizeBig(imagePP))
            {
                TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                return View();
            }


            //DB'de kayıtlı email kontrolü 
            if (_companyAdminRepository.Any(x => x.Email == siteAdmin.Email) || _employeeRepository.Any(x => x.Email == siteAdmin.Email) || _siteAdminRepository.Any(x => x.Email == siteAdmin.Email))
            {
                TempData["Msg"] = "Bu mail zaten kayıtlı";
                return View();
            }
            ViewBag.Hata = "Başarıyla eklendi...";

            _siteAdminRepository.Add(_siteAdminRepository.CreateBasic(siteAdmin, imagePP));
            _siteAdminRepository.ActivateMail(siteAdmin);

            return View();
        }


        [HttpGet]
        public IActionResult Update(Guid id)
        {
            SiteAdmin siteAdmin = _siteAdminRepository.GetByID(id);
            return View(siteAdmin);
        }

        [HttpPost]
        public IActionResult Update(SiteAdmin siteAdmin, IFormFile imagePP)
        {
            if (!(_superAdminRepository.IsImage(imagePP)))
            {
                TempData["IsImage"] = "Lütfen resim yükleyiniz";
                return View(siteAdmin);
            }
            if (_superAdminRepository.IsFileSizeBig(imagePP))
            {
                TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                return View(siteAdmin);
            }

            SiteAdmin siteAdminUdatedPicture = _siteAdminRepository.GetProfilPicture(siteAdmin, imagePP);
            SiteAdmin checkSiteAdmin = _siteAdminRepository.GetByID(siteAdmin.ID);

            //farklı email girdiyse bilgi maili
            if (siteAdminUdatedPicture.Email != checkSiteAdmin.Email)
            {
                if (_siteAdminRepository.Any(x => x.Email == siteAdminUdatedPicture.Email) || _employeeRepository.Any(x => x.Email == siteAdminUdatedPicture.Email) || _companyAdminRepository.Any(x => x.Email == siteAdminUdatedPicture.Email) || siteAdminUdatedPicture.Email == checkSiteAdmin.Email)
                {
                    TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                    return View(siteAdminUdatedPicture);
                }
                else
                    _siteAdminRepository.SendNewMailAddress(siteAdminUdatedPicture);
            }

            _siteAdminRepository.Update(siteAdminUdatedPicture);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public IActionResult PictureRemove(Guid id)
        {
            SiteAdmin siteAdmin = _siteAdminRepository.GetByID(id);
            _siteAdminRepository.RemoveImage(siteAdmin);

            return RedirectToAction("Index", "Home");
        }

        [Route("~/SuperAdministator/Home/Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            _siteAdminRepository.DeActivate(id);
            return RedirectToAction("Index", "Home");
        }

        //public IActionResult DeleteImage(Guid id)
        //{
        //    _siteAdminRepository.RemoveImage(_siteAdminRepository.GetByID(id));

        //    string url = string.Format($"~/SuperAdministator/Home/Update/{id}");
        //    return Json(Update(id));

        //}
    }
}
