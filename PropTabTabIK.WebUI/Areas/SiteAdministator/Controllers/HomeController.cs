using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.SiteAdministator.Controllers
{
    [Area("SiteAdministator")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {

        SuperAdminRepository _superAdminRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;
        EmployeeRepository _employeeRepository;
        PackageRepository _packageRepository;
        MailTemplateRepository _mailTemplateRepository;
        ContractRepository _contractRepository;
        public HomeController(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _contractRepository = new ContractRepository(context, httpContext);
            _superAdminRepository = new SuperAdminRepository(context, httpContext);
            _packageRepository = new PackageRepository(context, httpContext);
            _mailTemplateRepository = new MailTemplateRepository(context, httpContext);
            _employeeRepository = new EmployeeRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
        }
        public IActionResult Index()
        {
            ViewBag.FullName = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;

            List<CompanyAdmin> companyAdmins = _companyAdminRepository.GetDefault(x => x.SiteAdminID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID")));
            List<Package> packages = _packageRepository.GetAll();
            List<MailTemplate> mailTemplates = _mailTemplateRepository.GetDefault(x => x.UserID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID")));
            List<Employee> employees = _employeeRepository.GetAll();


            return View(Tuple.Create<List<CompanyAdmin>, List<Package>, List<MailTemplate>, List<Employee>>(companyAdmins, packages, mailTemplates, employees));


        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SiteAdmin siteAdmin, string email)
        {

            if (_siteAdminRepository.CheckCredential(email, siteAdmin.Password))
            {
                await HttpContext.SignInAsync(_siteAdminRepository.LoginClaims(siteAdmin));
                HttpContext.Session.SetString("SiteAdminID", _siteAdminRepository.GetByDefaultOutID(x => x.Email == siteAdmin.Email).ToString());


                //Bunu metoda çevirmeyi unutma
                if (_siteAdminRepository.FirstLogin(siteAdmin))//F- İlk girişi ise
                {
                    return RedirectToAction("NewPassword", "Home");
                }
                else//İlk girişi değilse
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Hata = "Hatalı Giriş Lütfen tekrar deneyin";
                return View(siteAdmin);
            }
        }

        #region PieChart
        public IActionResult PieResult()//pie chart görüntüsü sonucu
        {
            return Json(PieList());//PieList methodunu döndür.
        }
        public List<PieChart> PieList()
        {
            var chart = _employeeRepository.GetActive().GroupBy(x => x.CompanyAdminID).Select(x => new PieChart
            {
                // CompanyName = _companyRepository.CompanyAdmins.Where(y => y.ID == x.Key).Select(x => x.CompanyName).FirstOrDefault(),
                CompanyName = _companyAdminRepository.GetByDefault(y => y.ID == x.Key).CompanyName,//active olan companylerin isimleri
                Quantity = x.Count()//çalışan sayısı
            }).Distinct().ToList();
            return chart;
        }
        public IActionResult PieChartDetail()//pie chart görüntüsü sonucu
        {
            return View();//PieList methodunu döndür.
        }
        #endregion

        public IActionResult ForgotPassword(SiteAdmin siteAdmin)
        {
            #region SiteAdmin ViewBag's
            ViewBag.FullName = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.siteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(SiteAdmin siteAdmin, string email)
        {

            if (!_siteAdminRepository.CheckCredential(email, siteAdmin.Password))
            {
                ViewBag.Hata = "Böyle mail adresi sistemde kayıtlı değil";
            }
            else
            {
                siteAdmin.Password = _siteAdminRepository.GenerateToken(8);
                _siteAdminRepository.Update(siteAdmin);
                _siteAdminRepository.SendPasswordMail(siteAdmin);
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Settings()
        {

            #region SiteAdmin ViewBag'S
            ViewBag.FullName = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion


            return View(_siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))));
        }

        [HttpPost]
        public IActionResult Settings(SiteAdmin siteAdmin, IFormFile imagePP)
        {
            try
            {
                if (siteAdmin.Name.Length <= 2 && siteAdmin.Surname.Length <= 2)
                {
                    return View(siteAdmin);
                }

                if (!(_siteAdminRepository.IsImage(imagePP)))
                {
                    TempData["IsImage"] = "Lütfen resim yükleyiniz";
                    return View(siteAdmin);
                }
                if (_siteAdminRepository.IsFileSizeBig(imagePP))
                {
                    TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                    return View(siteAdmin);
                }

                SiteAdmin SiteAdminUpdate = _siteAdminRepository.GetProfilPicture(siteAdmin, imagePP);
                SiteAdmin siteAdmin1 = _siteAdminRepository.GetByDefault(x => x.ID == siteAdmin.ID);

                //farklı email girdiyse bilgi maili
                if (SiteAdminUpdate.Email != siteAdmin1.Email)
                {
                    if (_siteAdminRepository.Any(x => x.Email == siteAdmin.Email) || _employeeRepository.Any(x => x.Email == siteAdmin.Email) || _companyAdminRepository.Any(x => x.Email == siteAdmin.Email) || SiteAdminUpdate.Email == siteAdmin1.Email)
                    {
                        TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                        return View(SiteAdminUpdate);
                    }
                    else
                        _siteAdminRepository.SendNewMailAddress(SiteAdminUpdate);
                }
                TempData["success"] = "Başarıyla Güncellendi...";
                _siteAdminRepository.Update(SiteAdminUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KullaniciExists(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Settings", "Home");
        }



        private bool KullaniciExists(Guid id)
        {
            return _siteAdminRepository.Any(e => e.ID == id);
        }
        public IActionResult NewPassword()
        {
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");

            return View();
        }
        [HttpPost]
        public IActionResult NewPassword(SiteAdmin siteAdmin, string Password, string ConfirmPassword)
        {

            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");

            if (!String.IsNullOrEmpty(Password))
            {
                if (Password.Length >= 6)
                {
                    if (siteAdmin.Password == Password && ConfirmPassword == Password)
                    {
                        Guid id = Guid.Parse(HttpContext.Session.GetString("SiteAdminID"));
                        SiteAdmin newPasswordSiteAdmin = _siteAdminRepository.GetByDefault(x => x.ID == id);
                        newPasswordSiteAdmin.Password = Password;
                        newPasswordSiteAdmin.Status = Core.Enum.Status.Active;
                        _siteAdminRepository.Update(newPasswordSiteAdmin);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Hata"] = "Farklı Şifre Girdiniz. Lütfen tekrar deneyin";
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                TempData["Hata"] = "Sifre boş geçilemez!";
                return View();

            }

        }

        [Route("~/SiteAdministator/Home/PictureRemove/{id}")]
        public IActionResult PictureRemove(Guid id)
        {
            SiteAdmin siteAdmin = _siteAdminRepository.GetByID(id);
            _siteAdminRepository.RemoveImage(siteAdmin);


            return RedirectToAction("Settings", "Home");
        }

        public IActionResult Contracts()
        {
            #region SiteAdmin ViewBags
            ViewBag.FullName = _siteAdminRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");

            #endregion
            return View(_contractRepository.GetDefault(x => x.OwnerID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))));

        }

        [HttpGet]
        public IActionResult Detail(Guid id)
        {

            #region SiteAdmin ViewBag'S
            ViewBag.FullName = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).FullName;
            ViewBag.ProfilPicture = _siteAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("SiteAdminID"))).ProfilPicture;
            ViewBag.SiteAdminID = HttpContext.Session.GetString("SiteAdminID");
            #endregion



            return View(_contractRepository.GetByID(id));
        }

        [HttpGet]

        public IActionResult SignOut()//F- Çıkış yapar
        {
            HttpContext.Session.Remove(HttpContext.Session.GetString("SiteAdminID"));
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}
