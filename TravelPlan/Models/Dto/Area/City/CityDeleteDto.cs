namespace TravelPlan.Models.Dto.Area.City
{
    public class CityDeleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public int? CountryId { get; set; }
    }
}
