using PropTabTabIK.Core.Entity.Abstract;
using PropTabTabIK.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Core.Entity.Concrete
{
    public class SideEntity : IEntity<Guid>
    {
        public Guid ID { get; set; }

        public Status Status { get; set; }

        [Display(Name = "İsim")]
        [StringLength(100, ErrorMessage = "İsim 2 ile 100 karakter arasında olmalıdır.", MinimumLength = 2)]
        public string Name { get; set; }

    }
}
