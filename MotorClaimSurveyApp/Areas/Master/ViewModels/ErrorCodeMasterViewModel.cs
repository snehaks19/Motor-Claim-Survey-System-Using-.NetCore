using BusinessEntity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Master.ViewModels
{
    public class ErrorCodeMasterViewModel
    {
        public ErrorCodeMasterEntity ErrorCodeMasterEntity { get; set; }
        public string Mode { get; set; }
        public List<SelectListItem> ListErrType { get; set; } = new List<SelectListItem>();
    }
}
