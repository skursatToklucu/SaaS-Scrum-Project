using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Core.Enum
{
    public enum AdvancePaymentType
    {
        [Display(Name = "İş Avansı")]
        Work = 1,
        [Display(Name = "Maaş Avansı")]
        Salary = 2,
    }
}
