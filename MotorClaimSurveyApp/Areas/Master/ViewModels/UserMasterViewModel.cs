using BusinessEntity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotorClaimSurveyApp.Areas.Master.ViewModels
{
    public class UserMasterViewModel
    {
        public UserMasterEntity UserMasterEntity { get; set; }
        public string? Mode { get; set; }
        public List<SelectListItem> ListUserType { get; set; } = new List<SelectListItem>();


    }
}
