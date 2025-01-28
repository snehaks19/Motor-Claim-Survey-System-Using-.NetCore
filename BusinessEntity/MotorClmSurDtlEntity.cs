using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class MotorClmSurDtlEntity
    {
        public long SurdUid { get; set; }
        public long SurdSurUid { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string SurdItemCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string SurdType { get; set; }
        //[Required(ErrorMessage = "This field is required.")]
        public decimal SurdUnitPrice { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public decimal SurdQuantity { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public double SurdFcAmt { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public double SurdLcAmt { get; set; }
        public string? SurdRemarks { get; set; }
        public string? SurdCrBy { get; set; }
        public DateTime SurdCrDt { get; set; }
        public string? SurdUpBy { get; set; }
        public DateTime SurdUpDt { get; set; }
    }
}
