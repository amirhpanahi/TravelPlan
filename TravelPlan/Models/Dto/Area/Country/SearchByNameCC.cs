namespace TravelPlan.Models.Dto.Area.Country
{
    public class SearchByNameCC
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
