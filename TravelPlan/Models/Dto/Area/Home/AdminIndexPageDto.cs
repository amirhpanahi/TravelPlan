using Microsoft.EntityFrameworkCore;
using TravelPlan.Models.Entities;

namespace TravelPlan.Models.Dto.Area.Home
{
    public class AdminIndexPageDto
    {
        public int allHotel { get; set; }
        public int PublishHotel { get; set; }
        public int RejectHotel { get; set; }
        public int WatingHotel { get; set; }
        public int PercentOfWatitingHotel { get; set; }

        public int allTrip { get; set; }
        public int PublishTrip { get; set; }
        public int RejectTrip { get; set; }
        public int WatingTrip { get; set; }
        public int PercentOfWatitingTrip { get; set; }

        public int allRestaurant { get; set; }
        public int PublishRestaurant { get; set; }
        public int RejectRestaurant { get; set; }
        public int WatingRestaurant { get; set; }
        public int PercentOfWatitingRestaurant { get; set; }

        public int allComment { get; set; }
        public int PublishComment { get; set; }
        public int RejectComment { get; set; }
        public int WatingComment { get; set; }
        public int PercentOfWatitingComment { get; set; }

    }
}
