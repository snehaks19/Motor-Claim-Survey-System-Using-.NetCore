using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class MotorPolicyEntity
    {
        public long PolUid { get; set; }
        public string? PolNo { get; set; }
        public DateTime PolIssDt { get; set; } = DateTime.Now;

        [Required]
        public DateTime PolFmDt { get; set; }

        [Required]
        public DateTime PolToDt { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        public string PolAssrName { get; set; }

        [Required]
        public string PolAssrMobile { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        public string PolCurrCode { get; set; }

        [Required]
        public double PolGrossFcPrem { get; set; }

        [Required]
        public double PolGrossLcPrem { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string PolVehMake { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string PolVehModel { get; set; }

        [Required]
        public string PolVehChassisNo { get; set; }

        [Required]
        public string PolVehEngineNo { get; set; }

        [Required]
        public string PolVehRegnNo { get; set; }

        [Required]
        public double PolVehValue { get; set; }
        public string? PolApprStatus { get; set; }
        public DateTime? PolApprDt { get; set; }
        public string? PolApprBy { get; set; }
        public string? PolCrBy { get; set; }
        public DateTime? PolCrDt { get; set; }
        public string? PolUpBy { get; set; }
        public DateTime? PolUpDt { get; set; }
    }
}
