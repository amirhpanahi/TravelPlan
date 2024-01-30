using System.ComponentModel.DataAnnotations;

namespace TravelPlan.Models.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public bool IsSelected { get; set; }
        
    }
}
