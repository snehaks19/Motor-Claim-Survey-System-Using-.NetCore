using BusinessEntity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Transaction.ViewModels
{
    public class SurveyHeaderViewModel
    {
        public MotorClmSurHdrEntity MotorClmSurHdrEntity { get; set; }
        public MotorClmSurDtlEntity? MotorClmSurDtlEntity { get; set; }
        public string Mode { get; set; }
        public List<SelectListItem> ListCurrCode { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListLocation { get; set; } = new List<SelectListItem>();
    }
}
