using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class ContractRepository : SideRepository<Contract>
    {
        private readonly ProjectContext _context;
        private CompanyAdminRepository _companyAdminRepository;
        private IHttpContextAccessor _httpContext;
        public ContractRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _httpContext = httpContext;
            _context = context;
        }


        public Contract CreateContract(Guid OwnerID, Guid CompanyID, Guid PackageID)
        {
            Package package = _context.Packages.AsQueryable().Where(x => x.ID == PackageID).FirstOrDefault();

            Contract contract = new Contract();
            contract.Name = "";
            contract.Status = Core.Enum.Status.Active;
            contract.OwnerID = OwnerID;
            contract.CompanyID = CompanyID;
            contract.PackageID = PackageID;
            contract.StartDate = DateTime.Today;

            if (package.ExpireTime / 12 > 0)
                contract.EndDate = new DateTime(DateTime.Today.Year + package.ExpireTime / 12, DateTime.Today.Month + package.ExpireTime % 12, DateTime.Today.Day);
            if (package.ExpireTime / 12 < 0)
                contract.EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month + package.ExpireTime, DateTime.Today.Day);
            contract.Content = $"{_context.CompanyAdmins.AsQueryable().Where(x => x.ID == CompanyID).Select(x => x.CompanyName).FirstOrDefault()} şirket ile PropTabTab Yazılım arasında {contract.StartDate} tarihinde başlayan ve  {contract.EndDate} tarihine kadar sürecek olan, {package.EmployeeCount} kullanıcı sayısına sahip, {package.Price} ₺ değerindeki  {package.Name} isimli paket için  elektronik sözleşme imzalanmıştır.";

            _context.Add(contract);
            _context.SaveChanges();

            return contract;

        }
        public string AddressOfPropTabTab()
        {
            string adress = $"<strong>PropTabTab Boost Yazılım</strong> " +
                $"<em><br/>Warwick Building <br/>366 Queenstown Road <br/>Battersea, London <br/>SW11 8NJ</ em >" +
                $"<br/> <img style='width: 200px;' src='~/img/PropTabTabLogo.png'/>";

            return adress;
        }

        public string BaseUrl(CompanyAdmin entity)
        {

            string roleName = Enum.GetName(typeof(Core.Enum.Role), entity.Role.Value);

            var request = _httpContext.HttpContext.Request;
            var url = string.Format($"{request.Scheme}://{request.Host}/{roleName}/Home/Login");
            return url;
        }

        /// <summary>
        /// Contract iceren mail overloadu
        /// </summary>
        /// <param name="postedBy"></param>
        /// <param name="password"></param>
        /// <param name="entity"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="contract"></param>
        public void CreateMail(string postedBy, string password, CompanyAdmin entity, string subject, string content, string contract)
        {
            using (MailMessage mm = new MailMessage(postedBy, entity.Email))
            {

                mm.Subject = subject;
                string body = content;
                body += contract;

                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                NetworkCredential NetworkCred = new NetworkCredential(postedBy, password);
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                mm.Dispose();

            }
        }
        public void ContractMail(Contract contract)
        {
            CompanyAdmin companyAdmin = _context.CompanyAdmins.Where(x => x.ID == contract.CompanyID).FirstOrDefault();

            CreateMail("proptabtabhr@gmail.com", "baboost2021", companyAdmin,
                "Hoş Geldiniz",
                $"Merhaba {companyAdmin.FullName} " +
                $"<br/>Mail Adresiniz: {companyAdmin.Email} <br/>Parolanız: {companyAdmin.Password}<br />" +
                $"<a href = '{BaseUrl(companyAdmin)}'>Giriş yapmak için tıklayınız.</a><br /><br />",
                $"<br /><strong>Sözleşme Metni</strong>" +
                $"<br/>{contract.Content}" +
                $"<br/>{AddressOfPropTabTab()}");

        }

    }
}
