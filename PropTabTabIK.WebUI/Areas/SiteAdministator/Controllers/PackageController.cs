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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.SiteAdministator.Controllers
{
    [Area("SiteAdministator")]
    [Route("[area]/[controller]/[action]")]
    public class PackageController : Controller
    {
        PackageRepository _packageRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;
        public PackageController(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _packageRepository = new PackageRepository(context, httpContext);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
        }

        public IActionResult Index()
        {
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.siteAdminID = HttpContext.Session.GetString("SiteAdminID");
            SiteAdmin siteAdmin = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID")));
            return View(_packageRepository.GetActive());
        }

        [HttpGet]
        public IActionResult Update(Guid id)
        {
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminId"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.siteAdminID = HttpContext.Session.GetString("SiteAdminID");

            Package package = _packageRepository.GetByID(id);
            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Package package, IFormFile Photo)
        {
            //Burayı test et.
            string photo = _packageRepository.GetPhoto(package);

            if (Photo != null)
            {
                var uzanti = Path.GetExtension(Photo.FileName);
                var yeniResimAd = Guid.NewGuid() + uzanti;
                var yuklenecekYer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/package/" + yeniResimAd);
                var stream = new FileStream(yuklenecekYer, FileMode.Create);
                Photo.CopyTo(stream);
                package.Photo = yeniResimAd;
            }
            else
            {
                package.Photo = photo;//eğer yeni resim seçilmediyse eski resim alınır.
            }
            if (package.Price < 100 || package.Price > 100000 || package.Name.Length > 25 || package.ExpireTime > 60 || package.ExpireTime < 3 || package.EmployeeCount > 20000 || package.EmployeeCount < 10)
            {
                if (package.Name.Length > 25)
                {
                    ViewBag.Name = "Paket adının uzunluğu en fazla 25 karakter olmalı...";
                }
                if (package.Price < 100 || package.Price > 100000)
                {
                    ViewBag.Price = "Paket fiyatı en az 100 ₺, en fazla 100.000 ₺ olmalı...";
                }
                if (package.ExpireTime > 60 || package.ExpireTime < 3)
                {
                    ViewBag.ExpireTime = "Paket süresi en az 3 ay, en fazla 60 ay (5 yıl) olmalı...";
                }
                if (package.EmployeeCount > 20000 || package.EmployeeCount < 10)
                {
                    ViewBag.EmployeeCount = "Kullanıcı sayısı en az 10, en fazla 20000 olmalı...";
                }

                return View();
            }
            else
            {
                _packageRepository.Update(package);

                return RedirectToAction("Index", "Package");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Package package, IFormFile Photo)
        {
            if (Photo != null)
            {
                var uzanti = Path.GetExtension(Photo.FileName);
                var yeniResimAd = Guid.NewGuid() + uzanti;
                var yuklenecekYer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/package/" + yeniResimAd);
                var stream = new FileStream(yuklenecekYer, FileMode.Create);
                Photo.CopyTo(stream);
                package.Photo = yeniResimAd;
            }

            if (package.Price < 100 || package.Price > 100000 || package.Name.Length > 25 || package.ExpireTime > 60 || package.ExpireTime < 3 || package.EmployeeCount > 20000 || package.EmployeeCount < 10)
            {
                if (package.Name.Length > 25)
                {
                    TempData["Msg2"] = "Paket adının uzunluğu en fazla 25 karakter olmalı...";
                }
                if (package.Price < 100 || package.Price > 100000)
                {
                    TempData["Msg"] = "Paket fiyatı en az 100 ₺, en fazla 100.000 ₺ olmalı...";
                }
                if (package.ExpireTime > 60 || package.ExpireTime < 3)
                {
                    TempData["Msg3"] = "Paket süresi en az 3 ay, en fazla 60 ay (5 yıl) olmalı...";
                }
                if (package.EmployeeCount > 20000 || package.EmployeeCount < 10)
                {
                    TempData["Msg4"] = "Kullanıcı sayısı en az 10, en fazla 20000 olmalı...";
                }

                return RedirectToAction("Index", "Package");
            }
            if ((package.Price >= 100 && package.Price <= 100000 && package.Name.Length <= 25 && package.ExpireTime <= 60 && package.ExpireTime >= 3 && package.EmployeeCount <= 20000 && package.EmployeeCount >= 10))
            {
                package.Status = Core.Enum.Status.Active;
                _packageRepository.Add(package);
                return RedirectToAction("Index", "Package");
            }
            return RedirectToAction("Index", "Package");
        }


        [Route("~/SiteAdminArea/Package/Delete/{id?}")]
        public IActionResult Delete(Guid id)
        {
            Package package = _packageRepository.GetByID(id);

            var companyAdmin = _companyAdminRepository.GetByDefault(x => x.PackageID == package.ID);

            if (companyAdmin != null)
            {
                TempData["delete"] = "Silmek istediğiniz paket halen şirketler tarafından kullanıldığı için silemezsiniz.";
                return RedirectToAction("Index");
            }
            else
            {
                _packageRepository.Remove(package);
                return RedirectToAction("Index");
            }
            if (package == null)
            {
                return View(package);
            }



            return RedirectToAction("Index");

        }

    }
}
