using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Area.Users
{
    public class UserEditDto
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string FullName { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        [EmailAddress(ErrorMessageResourceName = nameof(ErrorMessage.EmailAddress), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        [RegularExpression(@"^\d{11}$", ErrorMessageResourceName = nameof(ErrorMessage.RegexPhoneNumber), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string PhoneNumber { get; set; }
        public string? PicAddress { get; set; }
        public string? PicTitle { get; set; }
        public string? PicAlt { get; set; }
        public IFormFile? File { get; set; }
    }
}
