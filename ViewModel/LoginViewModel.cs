using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace E_Indeks.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
