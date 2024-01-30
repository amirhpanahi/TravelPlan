using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Main.Account
{
	public class ForgetPasswordDto
	{
		[Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
		[EmailAddress(ErrorMessageResourceName = nameof(ErrorMessage.EmailAddress), ErrorMessageResourceType = typeof(ErrorMessage))]
		public string Email { get; set; }
	}
}
