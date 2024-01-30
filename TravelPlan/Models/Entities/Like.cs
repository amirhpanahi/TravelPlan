namespace TravelPlan.Models.Entities
{

    public class Like
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public StatusLike? StatusLike { get; set; }
        public TypeLike TypeLike { get; set; }
    }
    public enum StatusLike
    {
        Like,
        None
    }
    public enum TypeLike
    {
        Trip,
        Hotel,
        Restaurant
    }
}
