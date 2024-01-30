using Microsoft.AspNetCore.Identity;

namespace TravelPlan.Models.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? DateRegister { get; set; }
        public string? DateRegisterPresian { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? LastLoginDatePersian { get; set; }
        public string? PicAddress { get; set; }
        public string? PicAlt { get; set; }
        public string? PicTitle { get; set; }

        public string? BannerForProfile { get; set; }
        public string? AboutMe { get; set; }
    }
}
