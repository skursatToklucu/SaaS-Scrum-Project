using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SuperAdminRepository _superAdminRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;

        public HomeController(ILogger<HomeController> logger, ProjectContext context, IHttpContextAccessor httpContext)
        {

            _superAdminRepository = new SuperAdminRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SuperAdmin admin, string email)
        {
            if (_superAdminRepository.CheckCredential(email, admin.Password))
            {
                await HttpContext.SignInAsync(_superAdminRepository.LoginClaims(admin));
                HttpContext.Session.SetString("SuperAdminID", _superAdminRepository.GetByDefaultOutID(x => x.Email == admin.Email).ToString());
                return RedirectToAction("Index", "Home", new { area = "SuperAdministator" });
            }
            else
            {
                ViewBag.Hata = "Hatalı Giriş Lütfen tekrar deneyin";
                return View(admin);
            }
        }

        public IActionResult ForgotPassword()
        {
            TempData["Msg"] = "Lütfen Destek Birimi ile İletişime Geçiniz.";
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove(HttpContext.Session.GetString("SuperAdminID"));
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
