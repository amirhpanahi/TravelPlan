using TravelPlan.Models.Dto.Main.Comment;

namespace TravelPlan.Models.Dto.Main.ViewComponent
{
    public class ShowItemDto
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string IndexImage { get; set; }
        public string ImageAlt { get; set; }
        public string ImageTitle { get; set; }
        public string Description { get; set; }
        public string AddressAndDetails { get; set; }
        public string Summary { get; set; }
        public string Tags { get; set; }
        public string WriterName { get; set; }
        public string? PublishDatePersian { get; set; }
        public string? PublishDatePersianDay { get; set; }
        public string? PublishDatePersianMonth { get; set; }
        public string? PublishDatePersianYear { get; set; }
        public string? PublishDatePersianTime { get; set; }
        public int? CountOfLike { get; set; }
        public int? CountOfComment { get; set; }
        public string? LikeStatus { get; set; }
        public List<CommentListDto>? Comments { get; set; }
    }
}
