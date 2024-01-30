using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Resources;
using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Dto.Area.City
{
    public class CityCreateDto
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Name { get; set; }
        public int CountryId { get; set; }
        public List<SelectListItem>? Countries { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? IndexImageAddress { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        [Display(Name = "منتخب برای نمایش")]
        public bool IsSelected { get; set; }
        public IFormFile? IndexImageFile { get; set; }
    }
}
