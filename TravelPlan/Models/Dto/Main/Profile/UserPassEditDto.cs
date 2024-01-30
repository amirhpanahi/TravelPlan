using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Main.Profile
{
    public class UserPassEditDto
    {
        public string? Id { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
