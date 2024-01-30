using TravelPlan.Models.Entities;

namespace TravelPlan.Models.Dto.Area.Restaurant
{
    public class RestaurantDetailsDto
    {
        public string? Name { get; set; }
        public string? AddressAndDetails { get; set; }
        public string? Slug { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public string? Description { get; set; }
        public string? VideoAddress { get; set; }
        public string WriterName { get; set; }
        public bool IsSelected { get; set; }
        public RestaurantStatus? Status { get; set; }
        public string? RejectDatePersian { get; set; }
        public string? DeleteDatePersian { get; set; }
        public string? PublishDatePersian { get; set; }
        public string? RegisterDatePersian { get; set; }
        public string? RestaurantSummary { get; set; }
        public string? KeyWords { get; set; }
        public string? Tags { get; set; }

        public string? CountryName { get; set; }
        public string? CityName { get; set; }
    }
}
