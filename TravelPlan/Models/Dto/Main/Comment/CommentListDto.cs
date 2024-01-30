namespace TravelPlan.Models.Dto.Main.Comment
{
    public class CommentListDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? WriterName { get; set; }
        public int? ItemId { get; set; }
        public string? CommentText { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? RegisterDatePersian { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? ParentId { get; set; }
    }
}
