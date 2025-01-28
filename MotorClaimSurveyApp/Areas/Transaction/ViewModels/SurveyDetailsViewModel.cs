using BusinessEntity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Transaction.ViewModels
{
    public class SurveyDetailsViewModel
    {
        public MotorClmSurDtlEntity MotorClmSurDtlEntity { get; set; }
        public string? Mode { get; set; }
        public List<SelectListItem> ListItemCode { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListItemType { get; set; } = new List<SelectListItem>();
        public string? currencyCode { get; set; }
        public int? surUid { get; set; }
        public decimal lcAmount { get; set; }
    }
}
