using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Main.Account
{
    public class LoginDto
    {
		[Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
		[EmailAddress(ErrorMessageResourceName = nameof(ErrorMessage.EmailAddress), ErrorMessageResourceType = typeof(ErrorMessage))]
		public string Email { get; set; }

		[Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
		[DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "مرا به یاد بسپار")]
        public bool IsPersistent { get; set; } = false;
        public string ReturnUrl { get; set; }
    }
}
