namespace TravelPlan.Models.Dto.Area.Users
{
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
        public int AccessFailedCount { get; set; }
        public string DateRegister { get; set; }
        public string LastLoginDate { get; set; }
        public string DateRegisterPresian { get; set; }
        public string LastLoginDatePersian { get; set; }
    }
}
