using PropTabTabIK.DataAccess;
using PropTabTabIK.DataAccess.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropTabTabIK.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using System.Net;
using Microsoft.AspNetCore.Http;
using PropTabTabIK.WebUI.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.IO;//resim eklemek için
using Microsoft.AspNetCore.Authorization;
using PropTabTabIK.Entities.SideEntities;

namespace PropTabTabIK.WebUI.Areas.EmployeeArea.Controllers
{
    [Area("EmployeeArea")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {
        private EmployeeRepository _employeeRepository;
        private AllowRequestRepository _allowRequestRepository;
        private SiteAdminRepository _siteAdminRepository;
        private CompanyAdminRepository _companyAdminRepository;
        private PackageRepository _packageRepository;
        private AdvancePaymentRepository _advancePaymentRepository;

        public HomeController(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _employeeRepository = new EmployeeRepository(context, httpContext);
            _allowRequestRepository = new AllowRequestRepository(context, httpContext);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _advancePaymentRepository = new AdvancePaymentRepository(context, httpContext);

        }


        [HttpGet]

        public IActionResult Index()
        {
            #region Employee ViewBags
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;
            ViewBag.EmployeeID = HttpContext.Session.GetString("EmployeeID");
            #endregion

            ViewBag.AllowRequests = _allowRequestRepository.GetDefault(x => x.EmployeeID == Guid.Parse(HttpContext.Session.GetString("EmployeeID")));

            ViewBag.PaymentRequests = _advancePaymentRepository.GetDefault(x => x.EmployeeID == Guid.Parse(HttpContext.Session.GetString("EmployeeID")));
            ViewBag.Employee = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID")));

            return View();
        }




        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Employee employee, string email)
        {
            if (_employeeRepository.CheckCredential(email, employee.Password) && await _employeeRepository.IsCheckCaptcha())
            {//F- email, şifre ve captcha doğru ise giriş yapar.
                await HttpContext.SignInAsync(_employeeRepository.LoginClaims(employee));//F- Claims ile kimlik doğrulama yapar.
                HttpContext.Session.SetString("EmployeeID", _employeeRepository.GetByDefaultOutID(x => x.Email == employee.Email).ToString());

                if (_employeeRepository.FirstLogin(employee))//F- İlk girişi ise
                {
                    return RedirectToAction("NewPassword", "Home");
                }
                else//İlk girişi değilse
                    return RedirectToAction("Index", "Home");
            }
            else//F- Bilgiler doğru değilse
            {
                if (!_employeeRepository.CheckCredential(email, employee.Password))//F- Email veya şifre hatalı ise
                {
                    ViewBag.Hata = "Hatalı giriş lütfen tekrar deneyin...";
                    if (_employeeRepository.IsPasswordFail(employee, employee.Email, employee.Password))//F- 3 kere hatalı giriş olduysa
                    {
                        ViewBag.Hata = "3 kere hatalı giriş yaptınız. Hesabınız bloklandı!";
                        _employeeRepository.SendBlockedAccountMail(employee);//yeni şifre mail ile gönderilir.
                    }
                }
                else//F- Captcha doğrulanmadıysa
                {
                    ViewBag.HataCaptcha = "Captcha doğrulanmadı...";
                }
                return View(employee);
            }
        }

        [HttpGet]
        public IActionResult Settings()
        {
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;

            var employee = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID")));
            ViewBag.employeeID = HttpContext.Session.GetInt32("EmployeeID");

            if (employee == null)
            {
                return NotFound();
            }

            return View(_employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))));
        }


        //Çalışan bilgileri

        [HttpPost]
        public IActionResult Settings(Employee employee, IFormFile imagePP)
        {
            try
            {
                if (employee.Name.Length <= 2 && employee.Surname.Length <= 2)
                {
                    return View(employee);
                }

                if (!(_employeeRepository.IsImage(imagePP)))
                {
                    TempData["IsImage"] = "Lütfen resim yükleyiniz";
                    return View(employee);
                }
                if (_employeeRepository.IsFileSizeBig(imagePP))
                {
                    TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                    return View(employee);
                }

                Employee employeeUpdate = _employeeRepository.GetProfilPicture(employee, imagePP);
                Employee employee1 = _employeeRepository.GetByDefault(x => x.ID == employee.ID);

                //farklı email girdiyse bilgi maili
                if (employeeUpdate.Email != employee1.Email)
                {
                    if (_employeeRepository.Any(x => x.Email == employee.Email) || _companyAdminRepository.Any(x => x.Email == employee.Email) || _siteAdminRepository.Any(x => x.Email == employee.Email) || employeeUpdate.Email == employee1.Email)
                    {
                        TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                        return View(employeeUpdate);
                    }
                    else
                        _employeeRepository.SendNewMailAddress(employeeUpdate);
                }
                TempData["success"] = "Başarıyla Güncellendi...";
                _employeeRepository.Update(employeeUpdate);
                return RedirectToAction("Settings", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KullaniciExists(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool KullaniciExists(Guid id)
        {
            return _employeeRepository.Any(e => e.ID == id);
        }


        [Route("~/EmployeeArea/Home/PictureRemove/{id}")]
        public IActionResult PictureRemove(Guid id)
        {
            Employee employee = _employeeRepository.GetByID(id);
            _employeeRepository.RemoveImage(employee);


            return RedirectToAction("Settings", "Home");
        }

        [HttpGet]

        public async Task<IActionResult> SignOut()//F- Çıkış yapar
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }


        //Şifre unutma işlemi
        public IActionResult ForgotPassword()
        {
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;

            ViewBag.employeeID = HttpContext.Session.GetInt32("EmployeeID");
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(Employee employee)
        {
            if (_employeeRepository.CheckEmail(employee, employee.Email))
            {
                ViewBag.Hata = "Böyle mail adresi sistemde kayıtlı değil";
            }
            else
            {
                _employeeRepository.SendPasswordMail(employee);
                return RedirectToAction("Login", "Home");
            }
            return View();
        }



        public IActionResult NewPassword()
        {
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;

            ViewBag.employeeID = HttpContext.Session.GetInt32("EmployeeID");

            return View(new Employee());//header da resim görünmesi için

        }
        [HttpPost]
        public IActionResult NewPassword(Employee employee, string Password, string ConfirmPassword)
        {
            ViewBag.EmployeeID = HttpContext.Session.GetString("EmployeeID");

            if (!String.IsNullOrEmpty(Password))
            {
                if (Password.Length >= 6)
                {
                    if (employee.Password == Password && ConfirmPassword == Password)
                    {
                        Guid id = Guid.Parse(HttpContext.Session.GetString("EmployeeID"));
                        Employee newPasswordEmployee = _employeeRepository.GetByDefault(x => x.ID == id);
                        newPasswordEmployee.Password = Password;
                        newPasswordEmployee.Status = Core.Enum.Status.Active;
                        _employeeRepository.Update(newPasswordEmployee);
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
        public IActionResult Requests()
        {
            #region Employee's ViewBags
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;
            ViewBag.EmployeeID = HttpContext.Session.GetString("EmployeeID");
            #endregion

            ViewBag.AllowRequests = _allowRequestRepository.GetDefault(x => x.EmployeeID == Guid.Parse(HttpContext.Session.GetString("EmployeeID")));



            return View();
        }

        [HttpPost]
        public IActionResult Requests(AllowRequest allowRequest, DateTime start, DateTime end)
        {
            #region Employee ViewBags
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;
            ViewBag.EmployeeID = HttpContext.Session.GetString("EmployeeID");
            #endregion
            ViewBag.AllowRequests = _allowRequestRepository.GetDefault(x => x.EmployeeID == Guid.Parse(HttpContext.Session.GetString("EmployeeID")));

            allowRequest.StartDate = start;
            allowRequest.EndDate = end;
            allowRequest.IssueDate = DateTime.Now;

            if (_allowRequestRepository.IsContainDate(start, end))
            {
                TempData["DateInclude"] = "Girdiğiniz tarihlerde talep oluşturulmuş. Lütfen kontrol ediniz.";
                return View();
            }
            allowRequest.AllowTime = _allowRequestRepository.AllowTime(allowRequest.EndDate, allowRequest.StartDate);




            if (allowRequest.AllowTime > Convert.ToInt32(_employeeRepository.GetByDefault(x => x.ID == allowRequest.EmployeeID).AllowCount))
            {
                TempData["Hata"] = "Talebiniz yıllık izin hakkınızı geçiyor. Yeniden deneyiniz...";
                return View();
            }


            _allowRequestRepository.Add(allowRequest);
            allowRequest.RemainingAllowTime = _allowRequestRepository.RemainingAllowTime(allowRequest.ID);
            _allowRequestRepository.Update(allowRequest);



            List<AllowRequest> allowRequests = _allowRequestRepository.GetDefault(x => x.EmployeeID == allowRequest.EmployeeID && x.AllowType == allowRequest.AllowType && x.State == Core.Enum.State.Confirmed);

            if (allowRequests.Count > 1)
                _allowRequestRepository.Add(_allowRequestRepository.MergeAllowRequest(allowRequests));



            return View();
        }


        public IActionResult PaymentRequest()
        {
            #region Employee's ViewBags
            ViewBag.FullName = _employeeRepository.GetByID(Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).FullName;
            ViewBag.ProfilPicture = _employeeRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("EmployeeID"))).ProfilPicture;
            ViewBag.EmployeeID = HttpContext.Session.GetString("EmployeeID");
            #endregion

            ViewBag.AllowRequests = _allowRequestRepository.GetDefault(x => x.EmployeeID == Guid.Parse(HttpContext.Session.GetString("EmployeeID")));

            return View();
        }

        [HttpPost]
        public IActionResult PaymentRequest(AdvancePayment advancePayment)
        {

            advancePayment.IssueDate = DateTime.Now;


            if (advancePayment.Amount > _employeeRepository.GetByDefault(x => x.ID == advancePayment.EmployeeID).AdvancePaymentRight)
            {
                TempData["paymentRight"] = "Avans Hakkınızdan fazla değer girdiniz";
                return View();

            }
            else
            {
                _advancePaymentRepository.Add(advancePayment);
            }


            return View();
        }

    }
}
