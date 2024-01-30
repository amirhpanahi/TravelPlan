using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Dto.Area.Trip;

namespace TravelPlan.Models.Dto.Main.Search
{
    public class SearchByNameDto
    {
        public string? Name { get; set; }
        public List<HotelListDto>? ListHotels { get; set; }
        public List<TripListDto>? ListTrips { get; set; }
        public List<RestaurantListDto>? ListRestaurants { get; set; }
        public int? TotalOfFound { get; set; }

    }
}
