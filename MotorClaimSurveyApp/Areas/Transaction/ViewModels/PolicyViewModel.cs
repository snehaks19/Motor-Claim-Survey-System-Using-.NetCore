using BusinessEntity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Transaction.ViewModels
{
    public class PolicyViewModel
    {

        public MotorPolicyEntity objMotorPolicyEntity { get; set; }
        public List<SelectListItem> ListCurrCode { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListVehMake { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListVehModel { get; set; } = new List<SelectListItem>();
        public string Mode { get; set; }
        public string? currencyCode { get; set; }    
        public decimal fcAmount { get; set; }   
        
    }
}
