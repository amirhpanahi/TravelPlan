using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Area.Country
{
    public class CountryCreateDto
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public IFormFile? IndexImageFile { get; set; }
        public string? IndexImageAddress { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
    }
}
