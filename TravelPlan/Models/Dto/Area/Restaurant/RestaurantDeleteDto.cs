using Resources;

namespace TravelPlan.Models.Dto.Area.Restaurant
{
    public class RestaurantDeleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
    }
}
