namespace TravelPlan.Models.Dto.Main.Comment
{
    public class CommentEditDto
    {
        public int Id { get; set; }
        public string? CommentText { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
