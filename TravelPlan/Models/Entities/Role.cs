using Microsoft.AspNetCore.Identity;

namespace TravelPlan.Models.Entities
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
