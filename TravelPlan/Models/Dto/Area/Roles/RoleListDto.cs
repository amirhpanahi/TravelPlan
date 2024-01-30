using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Area.Roles
{
    public class RoleListDto
    {
        public string Id { get; set; }

        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Description { get; set; }
    }
}
