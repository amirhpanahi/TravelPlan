using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Dto.Main.Search;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class SearchItemViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _DbContext;
        public SearchItemViewComponent(UserManager<User> userManager, DatabaseContext dataBaseContext)
        {
            _userManager = userManager;
            _DbContext = dataBaseContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(string Name,string Type)
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            ViewBag.ListCity = _DbContext.Cities.Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var ListTrip = _DbContext.Trips.Where(x => x.Name.Contains(Name) || x.Description.Contains(Name) || x.AddressAndDetails.Contains(Name)
                            && x.TripStatus == TripStatus.Publish).Select(x => new TripListDto
                            {
                                Name = x.Name,
                                TripSummary = x.TripSummary,
                                IndexImage = x.IndexImage,
                                ImageAlt = x.ImageAlt,
                                ImageTitle = x.ImageTitle,
                                Slug = x.Slug,
                                CountryId = x.CountryId,
                                CityId = x.CityId

                            }).ToList();

            var ListResturant = _DbContext.Restaurants.Where(x => x.Name.Contains(Name) || x.Description.Contains(Name) || x.AddressAndDetails.Contains(Name)
                            && x.Status == RestaurantStatus.Publish).Select(x => new RestaurantListDto
                            {
                                Name = x.Name,
                                RestaurantSummary = x.RestaurantSummary,
                                IndexImage = x.IndexImage,
                                ImageAlt = x.ImageAlt,
                                ImageTitle = x.ImageTitle,
                                Slug = x.Slug,
                                CountryId = x.CountryId,
                                CityId = x.CityId

                            }).ToList();

            var ListHotel = _DbContext.Hotels.Where(x => x.Name.Contains(Name) || x.Description.Contains(Name) || x.AddressAndDetails.Contains(Name)
                            && x.Status == HotelStatus.Publish).Select(x => new HotelListDto
                            {
                                Name = x.Name,
                                HotelSummary = x.HotelSummary,
                                IndexImage = x.IndexImage,
                                ImageAlt = x.ImageAlt,
                                ImageTitle = x.ImageTitle,
                                Slug = x.Slug,
                                CountryId = x.CountryId,
                                CityId = x.CityId

                            }).ToList();

            var TotalOfFound = ListHotel.Count + ListResturant.Count + ListTrip.Count;

            var result = new SearchByNameDto
            {
                Name = Name,
                ListHotels = ListHotel,
                ListRestaurants = ListResturant,
                ListTrips = ListTrip,
                TotalOfFound = TotalOfFound,
            };

            return View(result);
        }
    }
}
