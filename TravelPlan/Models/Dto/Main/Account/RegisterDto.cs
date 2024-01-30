using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Main.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd),ErrorMessageResourceType = typeof(ErrorMessage))]
        public string FullName { get; set; }


        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        [EmailAddress(ErrorMessageResourceName = nameof(ErrorMessage.EmailAddress),ErrorMessageResourceType =typeof(ErrorMessage))]
        public string Email { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessageResourceName =nameof(ErrorMessage.RegexPhoneNumber),ErrorMessageResourceType =typeof(ErrorMessage))]
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessageResourceName = nameof(ErrorMessage.CompairePassword),ErrorMessageResourceType = typeof(ErrorMessage))]
        public string ConfirmPassword { get; set; }

        public string? PicTitle { get; set; }
        public string? PicAlt { get; set; }
    }
}
