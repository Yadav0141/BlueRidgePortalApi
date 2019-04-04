using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueRidgeUtility_BAL.Enums
{
    public enum Roles
    {
        [Display(Name = "Senior Director")]
        Senior_Director=1,
        [Display(Name = "Associate Director")]
        Associate_Director =2,
        [Display(Name = "Associate Technology Architect")]
        Associate_Technology_Architect =3,
        [Display(Name = "Design Analyst")]
        Design_Analyst =4,
        [Display(Name = "Software Design Engineer")]
        Software_Design_Engineer =5,
        [Display(Name = "DevOps Engineer")]
        DevOps_Engineer =6,
        [Display(Name = "QA Manager")]
        QA_Manager =7,
        [Display(Name = "Senior QA Analyst")]
        Senior_QA_Analyst =8,
        [Display(Name = "QA Analyst")]
        QA_Analyst =9,
        [Display(Name = "DevOps Architect")]
        DevOps_Architect =10
    }
}