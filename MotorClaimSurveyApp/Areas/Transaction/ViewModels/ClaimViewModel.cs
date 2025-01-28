using BusinessEntity;
using BusinessLayer;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Transaction.ViewModels
{
    public class ClaimViewModel
    {
        public MotorClaimEntity motorClaimEntity { get; set; }
        public MotorClmSurHdrEntity? motorClmSurHdrEntity { get; set; }
        public string Mode { get; set; }
        public List<SelectListItem> ListPolicyNo { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListVehMake { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListVehModel { get; set; } = new List<SelectListItem>();

    }
}
