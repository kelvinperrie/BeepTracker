using System.ComponentModel.DataAnnotations;

namespace BeepTracker.Blazor.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Please provide user name")]
        public string? UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide password")]
        public string? Password { get; set; }
    }
}
