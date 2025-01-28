using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class CodesMasterEntity
    {
        public string CmCode { get; set; }
        public string CmType { get; set; }
        public string? CmDesc { get; set; }
        public double? CmValue { get; set; }
        public string? CmParentCode { get; set; }
        public string? CmCrBy { get; set; }
        public DateTime? CmCrDt { get; set; }
        public string? CmUpBy { get; set; }
        public DateTime? CmUpDt { get; set; }
        public string CmActiveYn { get; set; } = "Y";
        public bool IsActive
        {
            get => CmActiveYn == "Y";
            set => CmActiveYn = value ? "Y" : "N";
        }

    }
}
