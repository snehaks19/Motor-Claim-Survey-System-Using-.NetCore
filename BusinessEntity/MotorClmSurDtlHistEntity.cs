using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class MotorClmSurDtlHistEntity
    {
        public long SurdhUid { get; set; }
        public int SurdhSrl { get; set; }
        public string SurdhAction { get; set; }
        public string SurdhItemCode { get; set; }
        public string SurdhType { get; set; }
        public int SurdhUnitPrice { get; set; }
        public int SurdhQuantity { get; set; }
        public double SurdhFcAmt { get; set; }
        public double SurdhLcAmt { get; set; }
        public string SurdhRemarks { get; set; }
        public string SurdhCrBy { get; set; }
        public DateTime SurdhCrDt { get; set; }
        public string SurdhUpBy { get; set; }
        public DateTime SurdhUpDt { get; set; }
    }
}
