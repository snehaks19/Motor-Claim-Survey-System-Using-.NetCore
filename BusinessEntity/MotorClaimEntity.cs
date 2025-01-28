using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class MotorClaimEntity
    {
        public long ClmUid { get; set; }
        public string? ClmNo { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string? ClmPolNo { get; set; }
        public DateTime ClmPolFmDt { get; set; }
        public DateTime ClmPolToDt { get; set; }
        public string? ClmPolAssrName { get; set; }
        public string? ClmPolAssrMob { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public DateTime ClmLossDt { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public DateTime ClmIntmDt { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public DateTime ClmRegDt { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string? ClmPolRepNo { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string? ClmPolRepDtl { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string? ClmLossDesc { get; set; }

        public string? ClmVehMake { get; set; }
        public string? ClmVehModel { get; set; }
        public string? ClmVehChassisNo { get; set; }
        public string? ClmVehEngineNo { get; set; }
        public string? ClmVehRegnNo { get; set; }
        public double ClmVehValue { get; set; }
        public string? ClmSurCrYn { get; set; }
        public string? ClmSurApprYn { get; set; }
        public string? ClmApprStatus { get; set; }
        public string? ClmSurNo { get; set; }
        public string? ClmCrBy { get; set; }
        public DateTime ClmCrDt { get; set; }
        public string? ClmUpBy { get; set; }
        public DateTime ClmUpDt { get; set; }
    }
}
