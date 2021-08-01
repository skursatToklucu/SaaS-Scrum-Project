using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.CompanyAdministator.Controllers
{
    [Area("CompanyAdministator")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class CompanyController : Controller
    {
        EmployeeRepository _employeeRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;
        PackageRepository _packageRepository;

        public CompanyController(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _employeeRepository = new EmployeeRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
        }


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(CompanyAdmin companyAdmin)
        {
            HttpContext.Session.SetString("CompanyAdminID", _companyAdminRepository.GetByDefaultOutID(x => x.Email == companyAdmin.Email).ToString());

            if (_companyAdminRepository.CheckCredential(companyAdmin.Email, companyAdmin.Password) && await _companyAdminRepository.IsCheckCaptcha())
            {
                await HttpContext.SignInAsync(_companyAdminRepository.LoginClaims(companyAdmin));
                HttpContext.Session.SetString("SiteAdminID", _siteAdminRepository.GetByDefaultOutID(x => x.Email == companyAdmin.Email).ToString());

                //Bunu metoda çevirmeyi unutma
                if (_companyAdminRepository.FirstLogin(companyAdmin))//F- İlk girişi ise
                {
                    return RedirectToAction("NewPassword", "Company");
                }
                else//İlk girişi değilse
                    return RedirectToAction("Index", "Home");
            }
            else//F- Bilgiler doğru değilse
            {
                if (!_companyAdminRepository.CheckCredential(companyAdmin.Email, companyAdmin.Password))//F- Email veya şifre hatalı ise
                {
                    ViewBag.Hata = "Hatalı giriş lütfen tekrar deneyin...";
                    if (_companyAdminRepository.IsPasswordFail(companyAdmin, companyAdmin.Email, companyAdmin.Password))//F- 3 kere hatalı giriş olduysa
                    {
                        ViewBag.Hata = "3 kere hatalı giriş yaptınız. Hesabınız bloklandı!";
                        _companyAdminRepository.SendBlockedAccountMail(companyAdmin);//yeni şifre mail ile gönderilir.
                    }
                }
                else//F- Captcha doğrulanmadıysa
                {
                    ViewBag.Hata = "Captcha doğrulanmadı...";
                }
                return View(companyAdmin);
            }
        }
        [HttpGet]
        public IActionResult EditCompany()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion
            CompanyAdmin companyAdmin = _companyAdminRepository.GetByDefault(sa => sa.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID")));
            return View(companyAdmin);
        }

        [HttpPost]
        public IActionResult EditCompany(CompanyAdmin companyAdmin, IFormFile imagePP, IFormFile logo)
        {
            try
            {
                //_companyAdminRepository.NullOrImage(companyAdmin, imagePP);
                //_companyAdminRepository.NullOrLogo(companyAdmin, logo);

                CompanyAdmin companyAdmin2 = _companyAdminRepository.NullOrLogo(companyAdmin, logo);
                companyAdmin2 = _companyAdminRepository.GetProfilPicture(companyAdmin, imagePP);

                CompanyAdmin companyAdmin1 = _companyAdminRepository.GetByDefault(x => x.ID == companyAdmin.ID);

                if (companyAdmin2.Email != companyAdmin1.Email)
                {
                    if (_companyAdminRepository.Any(x => x.Email == companyAdmin2.Email) || _employeeRepository.Any(x => x.Email == companyAdmin2.Email) || _siteAdminRepository.Any(x => x.Email == companyAdmin2.Email) || companyAdmin2.Email == companyAdmin1.Email)
                    {
                        TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                        return View(companyAdmin2);
                    }
                    else
                        _companyAdminRepository.SendNewMailAddress(companyAdmin2);
                }
                _companyAdminRepository.Update(companyAdmin2);
                return View(companyAdmin);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KullaniciExists(Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult ForgotPassword()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion

            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(CompanyAdmin companyAdmin)
        {
            if (!(_companyAdminRepository.Any(x => x.Email == companyAdmin.Email)))
            {
                ViewBag.Hata = "Böyle mail adresi sistemde kayıtlı değil";
            }
            else
            {
                _companyAdminRepository.SendPasswordMail(companyAdmin);
                return RedirectToAction("Login");
            }
            return View();
        }


        public IActionResult NewPassword()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;
            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;
            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion
            return View();
        }
        [HttpPost]
        public IActionResult NewPassword(CompanyAdmin companyAdmin, string Password, string ConfirmPassword)
        {

            ViewBag.SiteAdminID = HttpContext.Session.GetInt32("CompanyAdminID");
            if (companyAdmin.Password == Password && ConfirmPassword == Password)
            {
                Guid id = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
                CompanyAdmin newPasswordCompanyAdmin = _companyAdminRepository.GetByDefault(x => x.ID == id);
                newPasswordCompanyAdmin.Password = Password;
                newPasswordCompanyAdmin.Status = Core.Enum.Status.Active;
                _companyAdminRepository.Update(newPasswordCompanyAdmin);
                return RedirectToAction("Index", "Home", new { area = "CompanyAdministator" });
            }
            else
            {
                TempData["Hata"] = "Farklı Şifre Girdiniz. Lütfen tekrar deneyin";
                return View();
            }
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        private bool KullaniciExists(Guid id)
        {
            return _companyAdminRepository.Any(x => x.ID == id);
        }


    }
}
