using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class UserMasterEntity
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        [Compare("UserPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string? UserType { get; set; }
        public string? UserCrBy { get; set; }
        public DateTime? UserCrDt { get; set; }
        public string? UserUpBy { get; set; }
        public DateTime? UserUpDt { get; set; }
        public string? UserActiveYn { get; set; } = "Y";
        public bool IsActive
        {
            get => UserActiveYn == "Y";
            set => UserActiveYn = value ? "Y" : "N";
        }
    }
}
