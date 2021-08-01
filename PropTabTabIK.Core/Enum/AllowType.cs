using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PropTabTabIK.Core.Enum
{
    public enum AllowType
    {
        [Display(Name="Yıllık İzin")]
        annual = 1, //yillik izin
        [Display(Name = "Vefat İzni")]
        bereavement = 2, //vefat
        [Display(Name = "Doğum İzni")]
        maternity = 3, //dogum 
        [Display(Name = "Hastalık İzni")]
        disease = 4, //hastalik
        [Display(Name = "Askerlik İzni")]
        military = 5 //askerlik


    }
}
