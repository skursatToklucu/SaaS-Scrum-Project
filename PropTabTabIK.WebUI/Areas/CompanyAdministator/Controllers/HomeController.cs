using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Concrete;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI.Areas.CompanyAdministator.Controllers
{
    [Area("CompanyAdministator")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {
        EmployeeRepository _employeeRepository;
        SiteAdminRepository _siteAdminRepository;
        CompanyAdminRepository _companyAdminRepository;
        PackageRepository _packageRepository;
        AllowRequestRepository _allowRequestRepository;

        public HomeController(ProjectContext context, IHttpContextAccessor httpContext)
        {

            _employeeRepository = new EmployeeRepository(context, httpContext);
            _siteAdminRepository = new SiteAdminRepository(context, httpContext, _companyAdminRepository);
            _companyAdminRepository = new CompanyAdminRepository(context, httpContext, _packageRepository);
            _allowRequestRepository = new AllowRequestRepository(context, httpContext);
        }


        public IActionResult Index()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion


            return View();
        }

        public IActionResult EmployeesInfo()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion

            List<Employee> employees = _employeeRepository.GetDefault(x => x.CompanyAdminID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID")) && x.Status == Core.Enum.Status.Active);

            return View(employees);
        }

        public IActionResult CreateEmployee()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion

            return View();

        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee employee, IFormFile imagePP)
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion

            if (_employeeRepository.Any(x => x.Email == employee.Email) || _companyAdminRepository.Any(x => x.Email == employee.Email) || _siteAdminRepository.Any(x => x.Email == employee.Email))
            {
                TempData["Msg"] = "Bu mail zaten kayıtlı";
                return View(employee);
            }
            if (!(_employeeRepository.IsImage(imagePP)))
            {
                TempData["IsImage"] = "Lütfen resim yükleyiniz";
                return View(employee);
            }
            if (_companyAdminRepository.IsFileSizeBig(imagePP))
            {
                TempData["image"] = "2MB'dan büyük resim yükleyemezsiniz";
                return View(employee);
            }
            ViewBag.Success = "Başarıyla eklendi...";

            _employeeRepository.Add(_employeeRepository.CreateBasic(employee, imagePP));
            _employeeRepository.ActivateMail(employee);

            return RedirectToAction("CreateEmployee", "Home");
        }



        public IActionResult EditEmployee(Guid id)
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion


            return View(_employeeRepository.GetByID(id));
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee employee, IFormFile imagePP)
        {

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

            if (employeeUpdate.Email != employee1.Email)
            {
                if (_companyAdminRepository.Any(x => x.Email == employeeUpdate.Email) || _employeeRepository.Any(x => x.Email == employeeUpdate.Email) || _siteAdminRepository.Any(x => x.Email == employeeUpdate.Email) || employeeUpdate.Email == employee1.Email)
                {
                    TempData["emailCheck"] = "Böyle bir email zaten kayıtlı";
                    return View(employeeUpdate);
                }
                else
                    _employeeRepository.SendNewMailAddress(employeeUpdate);
            }
            _employeeRepository.Update(employeeUpdate);


            return RedirectToAction("Index", "Home");
        }


        [Route("~/CompanyAdministator/Home/Delete/{id?}")]
        public IActionResult Delete(Guid id)
        {
            Employee employee = _employeeRepository.GetByID(id);
            employee.Status = Core.Enum.Status.Deleted;
            _employeeRepository.Update(employee);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Requests()
        {
            #region Company Admin's Viewbags
            ViewBag.FullName = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).FullName;

            ViewBag.ProfilPicture = _companyAdminRepository.GetByDefault(x => x.ID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"))).ProfilPicture;

            ViewBag.CompanyAdminID = Guid.Parse(HttpContext.Session.GetString("CompanyAdminID"));
            #endregion

            List<AllowRequest> allow = _allowRequestRepository.GetAll();//bütün izinler
            List<Employee> employeesTemp = _employeeRepository.GetDefault(x => x.CompanyAdminID == Guid.Parse(HttpContext.Session.GetString("CompanyAdminID")));//o şirketin çalışanları
            List<Employee> employeesAllow = new List<Employee>();

            foreach (var item in allow)
            {
                employeesAllow.Add(_employeeRepository.GetByDefault(x => x.ID == item.EmployeeID));
            }
            ViewBag.Employee = employeesAllow;

            return View(allow);
        }
        private bool KullaniciExists(Guid v)
        {
            return _employeeRepository.Any(x => x.ID == v);
        }

        [Route("~/CompanyAdministator/Home/DeleteAllow/{id?}")]
        public IActionResult DeleteAllow(Guid id)
        {
            AllowRequest allowRequest = _allowRequestRepository.GetByID(id);
            allowRequest.State = Core.Enum.State.Denied;

            _allowRequestRepository.Update(allowRequest);
            return RedirectToAction("Requests", "Home");
        }

        [Route("~/CompanyAdministator/Home/ConfirmedAllow/{id?}")]
        public IActionResult ConfirmedAllow(Guid id)
        {
            AllowRequest allowRequest = _allowRequestRepository.GetByID(id);
            allowRequest.State = Core.Enum.State.Confirmed;
            _allowRequestRepository.Update(allowRequest);

            Employee employee = _employeeRepository.GetByDefault(x => x.ID == allowRequest.EmployeeID);
            employee.AllowCount -= Convert.ToByte(allowRequest.AllowTime);

            _employeeRepository.Update(employee);

            return RedirectToAction("Requests", "Home");
        }
    }
}
