using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Abstract;
using PropTabTabIK.Entities.Entities;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class Repository<T> : IRepository<T> where T : CoreEntity
    {

        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;

        public Repository(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        //PropTabTab Adres Bilgisi
        public string AddressOfPropTabTab()
        {
            string adress = $"   <strong>PropTabTab Boost Yazılım</strong> " +
                $"<em><br/>Warwick Building <br/>366 Queenstown Road <br/>Battersea, London <br/>SW11 8NJ</ em >" +
                $"<br/> <img style='width: 200px;' src='~/img/PropTabTabLogo.png'/>";

            return adress;
        }

        // PoweredBy Kartal Kaan Akdogan
        #region Main Methods

        /// <summary>
        /// Gelen ID ye Ait Entitynin Status durumunu Active olarak değiştirip, update eder.
        /// </summary>
        /// <param name="id">Entitynin ID si</param>
        /// <returns></returns>
        public bool Activate(Guid id)
        {
            T activated = GetByID(id);
            activated.Status = Core.Enum.Status.Active;
            return Update(activated);
        }

        /// <summary>
        /// Gelen ID ye Ait Entitynin Status durumunu deactive olarak değiştirip, update eder.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeActivate(Guid id)
        {
            T deactivated = GetByID(id);
            deactivated.Status = Core.Enum.Status.Deactive;
            return Update(deactivated);
        }

        /// <summary>
        /// Gelen Entity'nin ilk girişi olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool FirstLogin(T item)
        {
            T first = GetByDefault(x => x.Email == item.Email);
            if (first.Status == Core.Enum.Status.None)
            {
                Activate(first.ID);
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Gelen Entity'i Database'e ekler.
        /// </summary>
        /// <param name="item">Model Alır</param>
        /// <returns></returns>
        public bool Add(T item)
        {
            _context.Set<T>().Add(item);
            return Save() > 0;

        }

        /// <summary>
        /// Liste halinde modelleri database'e aktarır.
        /// </summary>
        /// <param name="items">Model alır</param>
        /// <returns></returns>
        public bool Add(List<T> items)
        {
            _context.Set<T>().AddRange(items);
            return Save() > 0;
        }

        /// <summary>
        /// Sorguya göre veritabanında var mı yok mu diye kontrol eder. Bool döndürür.
        /// </summary>
        /// <param name="exp">Sorgu mesela (x => x.falanfilan)</param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> exp) => _context.Set<T>().Any(exp);


        /// <summary>
        /// Databasede bulunan Statusü Active olan entityleri listeler
        /// </summary>
        /// <returns></returns>
        public List<T> GetActive() => _context.Set<T>().Where(x => x.Status == Core.Enum.Status.Active).ToList();
        

        /// <summary>
        /// Ne var Ne yok getirir :)
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll() => _context.Set<T>().ToList();


        /// <summary>
        /// Sorguya göre Entityi döndürür.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public T GetByDefault(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp).FirstOrDefault();



        /// <summary>
        /// Databasede bulunan Statusü Active olan entityi dndürür. PieChart için method
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public T GetByDefaultActive(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp => exp.Status == Core.Enum.Status.Active).FirstOrDefault();
           



        /// <summary>
        /// Sorguya göre Entitynin ID'sini döndürür.
        /// </summary>
        /// <param name="exp">Sorgu</param>
        /// <returns></returns>
        public Guid GetByDefaultOutID(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp).Select(x => x.ID).FirstOrDefault();




        /// <summary>
        /// Gelen ID'ye göre o ID'ye ait Entity'i döndürür.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetByID(Guid id)
        {
            return _context.Set<T>().Find(id);
        }


        /// <summary>
        /// Sorguya göre Liste döndürür Entity içerir.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public List<T> GetDefault(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp).ToList();

        /// <summary>
        /// Gelen Entitynin Statusünü Deleted olarak değiştirir.Silinmiş gibi yapar.
        /// </summary>
        /// <param name="item">Entity</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            item.Status = Core.Enum.Status.Deleted;
            return Update(item);
        }

        /// <summary>
        /// Gelen ID'yi göre Entitynin statusünü Deleted yapar. 
        /// Transaction :Birbirini izleyen işlemlerin herhangi birinde hata olması durumunda yapılan tüm işlemlerim geri alınmasını sağlar.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(Guid id)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    T deleted = GetByID(id);
                    deleted.Status = Core.Enum.Status.Deleted;
                    ts.Complete();
                    return Update(deleted);
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// Sorguya göre Hepsini Siler
        /// Transaction :Birbirini izleyen işlemlerin herhangi birinde hata olması durumunda yapılan tüm işlemlerim geri alınmasını sağlar.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool RemoveAll(Expression<Func<T, bool>> exp)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var collection = GetDefault(exp);
                    int count = 0;


                    foreach (var item in collection)
                    {
                        item.Status = Core.Enum.Status.Deleted;
                        bool opResult = Update(item);
                        if (opResult) count++;
                    }

                    if (collection.Count == count) ts.Complete();
                    else return false;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        /// <summary>
        /// Veritabanına kaydeder.
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            return _context.SaveChanges();
        }


        /// <summary>
        /// Gelen Entityi Günceller
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Update(T item)
        {
            try
            {
                _context.Set<T>().Update(item);
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;

            }
        }

        /// <summary>
        /// Liste halinde gelen entitileri toplu gunceller.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool UpdateRange(List<T> entities)
        {
            try
            {
                _context.Set<T>().UpdateRange(entities);
                return Save() > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// İlgili entitynin Profil Resmini getirir.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string GetPicture(T entity)

        {
            return _context.Set<T>().Where(x => x.ID == entity.ID).Select(x => x.ProfilPicture).FirstOrDefault();
        }


        #endregion


        // PoweredBy PropTabTab Team 

        /// <summary>
        /// Random güçlü şifre oluşturur
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GenerateToken(int length)
        {
            using (RNGCryptoServiceProvider cryptRNG = new RNGCryptoServiceProvider())
            {
                byte[] tokenBuffer = new byte[length];
                cryptRNG.GetBytes(tokenBuffer);
                return Convert.ToBase64String(tokenBuffer);
            }
        }

        public bool IsImage(IFormFile postedFile)
        {
            if (postedFile == null)
                return true;
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (
                !string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jfif", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jfif", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Resim yüklemk için bir resim yolu oluşturur
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public string ImageUpload(IFormFile image)
        {
            string imagePath = string.Empty;

            if (image.ContentType.Contains("image"))
            {
                if (image.Length <= 2097152)
                {
                    var path = Path.GetExtension(image.FileName);
                    var newPictureName = Guid.NewGuid() + path;
                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProfilPictures\\" + newPictureName);
                    var stream = new FileStream(location, FileMode.Create);
                    image.CopyTo(stream);
                    imagePath = location.Substring(location.IndexOf("\\ProfilPictures\\"));
                }
            }
            return imagePath;
        }


        /// <summary>
        /// Dosyanın büyüklüğünü kontrol eder 2MB dan buyuk olmamalı
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsFileSizeBig(IFormFile image)
        {
            if (image == null)
                return false;
            if (image.Length > 2097152)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Bozuk Method
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string SetDefaultPicture(T entity)
        {
            List<string> _BackgroundColours = new List<string> { "3C79B2", "FF8F88", "6FB9FF", "C0CC44", "AFB28C" };
            var avatarString = string.Format("{0}{1}", entity.Name[0], entity.Surname[0]).ToUpper();
            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 48, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 192, 192), stringFormat);
            graphics.Flush();


            var memoryStream = new MemoryStream();
            bmp.Save(memoryStream, ImageFormat.Png);

            byte[] byteImage = memoryStream.ToArray();
            string imagePath = Convert.ToBase64String(byteImage);

            return imagePath;

        }



        /// <summary>
        /// Firma logolarini yuklemek icin method
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public string LogoUpload(IFormFile image)
        {
            string imagePath = string.Empty;
            if (image.ContentType.Contains("image"))
            {
                if (image.Length <= 2097152)
                {
                    var path = Path.GetExtension(image.FileName);
                    var newPictureName = Guid.NewGuid() + path;
                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Logos\\" + newPictureName);
                    var stream = new FileStream(location, FileMode.Create);
                    image.CopyTo(stream);
                    imagePath = location.Substring(location.IndexOf("\\Logos\\"));
                }
            }

            return imagePath;
        }

        /// <summary>
        /// İlgili Entitynin resmini kaldırır
        /// <br/> Dikkat! Update methodu icerir.
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveImage(T entity)
        {
            entity.ProfilPicture = "";
            Update(entity);
        }


        public string BaseUrl(T entity)
        {

            string roleName = Enum.GetName(typeof(Core.Enum.Role), entity.Role.Value);

            var request = _httpContext.HttpContext.Request;
            var url = string.Format($"{request.Scheme}://{request.Host}/{roleName}/Home/Login");
            return url;
        }


        /// <summary>
        /// Mail için using bloğu 
        /// </summary>
        /// <param name="postedBy">Kim Tarafından</param>
        /// <param name="password">Şifre</param>
        /// <param name="entity">Gönderilecek Entıty</param>
        /// <param name="subject">Konu</param>
        /// <param name="content">İçerik</param>
        public void CreateMail(string postedBy, string password, T entity, string subject, string content)
        {
            using (MailMessage mm = new MailMessage(postedBy, entity.Email))
            {

                mm.Subject = subject;
                string body = content;

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



        #region Mail İşlemleri

        /// <summary>
        /// Mesaj Şablonu içeren mail göndermeye yarayan method.
        /// </summary>
        /// <param name="entity">Adminlerden birini seç ama SuperAdmin olmasın :)</param>
        /// <param name="templateId">Mail Şablonuna ait ID</param>
        public void SendMail(T entity, Guid templateId)
        {
            string getMailContent = _context.MailTemplates.Where(x => x.ID == templateId).Select(x => x.Content).FirstOrDefault();
            string getSubject = _context.MailTemplates.Where(x => x.ID == templateId).Select(x => x.Subject).FirstOrDefault();
            CreateMail("proptabtabhr@gmail.com", "baboost2021", entity, getSubject, getMailContent);
        }

        ///<summary>
        ///Şifre 3 kez yanlış girildiğinde yeni şifre yollamaya yarayan method.
        ///Birsey döndürmez
        ///</summary>
        public void SendBlockedAccountMail(T entity)
        {
            CreateMail(
                "proptabtabhr@gmail.com",
                "baboost2021",
                entity,
                "Hesabınız Bloke Edimiştir",
                $" Merhaba {entity.FullName} , <br/> Lütfen aşağıdaki yeni şifreniz ile giriş yapınız. <br/> E-mailiniz: {entity.Email} <br/> Parolanız: {entity.Password} <br /> <a href ='{BaseUrl(entity)}'>Giriş Sayfasına gitmek için tıklayınız.</a><br /><br /> Teşekkürler" +
                $"<br/>{AddressOfPropTabTab()}");
        }

        ///<summary>
        ///Yeni Şirket için paket bilgisi ve hoşgeldin maili yollamaya yarayan meethod.
        ///Birsey döndürmez
        ///</summary>
        public void ActivateCompanyMail(T entity, Package package)
        {
            CreateMail(
               "proptabtabhr@gmail.com",
               "baboost2021",
               entity,
               "PropTabTab İnsan kaynakları Yazılımına Hoşgeldiniz",
               $"Merhaba {entity.FullName} , " +
               $"<br/> Değerli üyemiz, " +
               $"<br/> <strong>{package.Name}</strong> isimli paketi satın aldığınız için teşekkür ederiz." +
               $"<br/> Paket Bilgileriniz : " +
               $"<br/> " +
               $"<ul><li>Paket ismi: {package.Name} </li><li>Paket Ücreti: {package.Price} ₺ </li><li>Paketin sınırlı olduğu çalışan sayısı: {package.EmployeeCount}</li><li>Paketin Süresi: {package.ExpireTime} Ay</li> </ul> " +
               $"<br/> Paket bitimine yakın bilgilendirileceksiniz. Dilerseniz istediğiniz zaman paketinizi yükseltebilirsiniz." +
               $"<br/> Giriş Bilgileriniz: " +
               $"<br/> E-mailiniz: {entity.Email} " +
               $"<br/> Parolanız: {entity.Password} " +
               $"<br /> <a href ='{BaseUrl(entity)}'>Giriş Sayfasına gitmek için tıklayınız.</a><br />" +
               $"<br /> <br/> <br/> Bizi tercih ettiğiniz için Teşekkür Ederiz" +
               $"<br/>{AddressOfPropTabTab()}");
        }

        ///<summary>
        ///Parola Unuttum hareketı için bilgi maili  ve yeni parola göndermeye yarayan method.
        ///Birsey döndürmez
        ///</summary>
        public void SendPasswordMail(T entity)
        {
            CreateMail(
                "proptabtabhr@gmail.com",
                "baboost2021",
                entity,
                "Yeni Şifre Maili",
                $"Merhaba {entity.FullName} , <br/>Yeni Parolanız: {entity.Password} <br /> <a href ='{BaseUrl(entity)}'>Giriş Sayfasına gitmek için tıklayınız.</a><br /><br /> Teşekkürler" +
                $"<br/>{AddressOfPropTabTab()}");
        }


        ///<summary>
        ///Mail Değişikliği için bilgi maili göndermeye yarayan method.
        ///Birsey döndürmez
        ///</summary>
        public void SendNewMailAddress(T entity)
        {
            CreateMail(
                "proptabtabhr@gmail.com",
                "baboost2021",
                entity,
                "Yeni Mail Adresiniz",
                $"{entity.Email} , <br/><a href ='{BaseUrl(entity)}'>Giriş Sayfasına gitmek için tıklayınız.</a><br /><br /> Teşekkürler" +
                $"<br/>{AddressOfPropTabTab()}");
        }

        /// <summary>
        /// SiteAdmin ve Calisan icin tasarlanmıs method
        /// </summary>
        /// <param name="entity"></param>
        public void ActivateMail(T entity)
        {
            CreateMail("proptabtabhr@gmail.com", "baboost2021", entity,
                "Hoş Geldiniz",
                $"Merhaba { entity.FullName} " +
                $"<br/>Mail Adresiniz: {entity.Email} <br/>Parolanız: {entity.Password}<br />" +
                $"<a href = '{BaseUrl(entity)}'>Giriş yapmak için tıklayınız.</a><br /><br />" +
                $"<br />Teşekkürler" +
                $"{AddressOfPropTabTab()}");
        }

        #endregion


        /// <summary>
        /// Eger DB de resim varsa guncellerken null gecildiyse eski resmi tutar.
        /// <br/> Dikkat! Update methodu icerir.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="imagePP"></param>
        public T GetProfilPicture(T entity, IFormFile imagePP)
        {
            if (imagePP == null) entity.ProfilPicture = GetPicture(entity);
            else entity.ProfilPicture = ImageUpload(imagePP);
            return entity;
        }


        /// <summary>
        /// 3 kere yanlıs girilir ise true döndürür.
        /// <br/> Dikkat! Update methodu icerir.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsPasswordFail(T entity, string email, string password)
        {
            if (CheckEmail(entity, email))
            {
                if (CheckPassword(entity, password))
                {
                    entity.ErrorLogin++;
                    Update(entity);

                    if (entity.ErrorLogin > 2)
                    {
                        entity.Status = Core.Enum.Status.None;
                        entity.Password = GenerateToken(8);
                        entity.ErrorLogin = 0;
                        Update(entity);
                        return true;
                    }

                }
            }
            return false;
        }



        /// <summary>
        /// Capthca, EMail, Şifre ve Aktif mi diye kontrol eder.
        /// <br/> string tek tek hataların acıklamasını döndürür.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> IsAllCheckForLogin(T entity)
        {
            if (await IsCheckCaptcha() && CheckCredential(entity.Email, entity.Password) && entity.Status != Core.Enum.Status.Active)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Captcha kontrol eden method.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsCheckCaptcha()
        {
            var postData = new List<KeyValuePair<string, string>>()
             {
                      new KeyValuePair<string, string>("secret", "6LeVJe8aAAAAAE0LPiaGkVdKrWWLf-dfjvnbzZBi"),
                      new KeyValuePair<string, string>("remoteip", _httpContext.HttpContext.Connection.RemoteIpAddress.ToString()),
                      new KeyValuePair<string, string>("response", _httpContext.HttpContext.Request.Form["g-recaptcha-response"])
             };

            var client = new HttpClient();
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(postData));

            var o = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            return (bool)o["success"];
        }


        /// <summary>
        /// Kimlik doğrulamak için ClaimsPrincipal döndüren method.
        /// <br/>Kullanım Şekli : await HttpContext.SignInAsync(LoginClaims(entity))
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ClaimsPrincipal LoginClaims(T entity)
        {
            var claims = new List<Claim>//Kimlik doğrulama
                {
                    new Claim(ClaimTypes.Name, entity.Email)
                };
            var userIdentity = new ClaimsIdentity(claims, "Login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            return principal;
        }


        /// <summary>
        /// Email ve Sifre dogrulugunu kontrol eder.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckCredential(string email, string password)
        {
            return Any(x => x.Email == email && x.Password == password);
        }

        /// <summary>
        /// TC NO için bir konrol methodu bool döndürür
        /// </summary>
        /// <param name="tcNo"></param>
        /// <returns></returns>
        public bool IsIdentityValid(string tcNo)
        {
            bool returnvalue = false;
            if (tcNo.Length == 11)
            {
                Int64 ATCNO, BTCNO, TcNo;
                long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;
                TcNo = Int64.Parse(tcNo);
                ATCNO = TcNo / 100;
                BTCNO = TcNo / 100;
                C1 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C2 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C3 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C4 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C5 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C6 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C7 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C8 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C9 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);
                returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
            }

            return returnvalue;
        }


        /// <summary>
        /// Mail dogrulugunu kontrol eder
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmail(T entity, string email)
        {
            if (entity.Email == email)
                return true;
            else
                return false;
        }



        /// <summary>
        /// Şifre dogrulugunu kontrol eder.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPassword(T entity, string password)
        {
            if (entity.Password == password)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Resim null ise Default avatar ekler degilse yuklenen resmi yukler
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="imagePP"></param>
        /// <returns></returns>
        public T NullOrImage(T entity, IFormFile imagePP)
        {
            if (imagePP == null)
            {
                entity.ProfilPicture = "https://www.deracevanmijnleven.nl/wp-content/uploads/2021/01/Blank-Avatar-Image.jpg";
            }
            else
            {

                entity.ProfilPicture = ImageUpload(imagePP);

            }

            return entity;
        }


    }

}


