using DataAnnotationsExtensions;
using PropTabTabIK.Core.Entity.Abstract;
using PropTabTabIK.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PropTabTabIK.Core.Entity.Concrete
{
    public class CoreEntity : IEntity<Guid>
    {
        public Guid ID { get; set; }

        public Status Status { get; set; }

        public Role? Role { get; set; }

        [Required(ErrorMessage = "Email boş geçilemez!")]
        
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş geçilemez!")]
        [Display(Name = "Şifre")]
        [StringLength(100, ErrorMessage = "Şifreniz minimum 6 karakterden oluşmalıdır.", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "İsim")]
        [StringLength(50, ErrorMessage = "İsminiz 2 ile 50 karakter arasında olmalıdır.", MinimumLength = 2)]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        [StringLength(50, ErrorMessage = "Soyisminiz 2 ile 50 karakter arasında olmalıdır.", MinimumLength = 2)]
        public string Surname { get; set; }

        [Display(Name = "Profil Resmi")]
        public string ProfilPicture { get; set; }

        public byte ErrorLogin { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return Name + " " + Surname; }
        }

    }
}
