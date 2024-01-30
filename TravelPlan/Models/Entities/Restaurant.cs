namespace TravelPlan.Models.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? AddressAndDetails { get; set; }
        public int CountryId { get; set; }
        public string? Slug { get; set; }
        public int CityId { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public string? Description { get; set; }
        public string? VideoAddress { get; set; }
        public string WriterId { get; set; }
        public bool IsSelected { get; set; }
        public RestaurantStatus? Status { get; set; }
        public string? RejectDate { get; set; }
        public string? RejectDatePersian { get; set; }
        public string? DeleteDate { get; set; }
        public string? DeleteDatePersian { get; set; }
        public string? PublishDate { get; set; }
        public string? PublishDatePersian { get; set; }
        public string? RegisterDate { get; set; }
        public string? RegisterDatePersian { get; set; }
        public string? RestaurantSummary { get; set; }
        public string? KeyWords { get; set; }
        public string? Tags { get; set; }
    }
    public enum RestaurantStatus
    {
        Publish,
        Delete,
        WaitingForConfirm,
        RejectedByAdmin
    }

}
