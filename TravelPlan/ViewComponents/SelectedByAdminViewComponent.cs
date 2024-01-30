using Microsoft.AspNetCore.Mvc;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class SelectedByAdminViewComponent : ViewComponent
    {
        private readonly DatabaseContext _dbContext;
        public SelectedByAdminViewComponent(DatabaseContext dataBaseContex)
        {
            _dbContext = dataBaseContex;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.ListCountry = _dbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            ViewBag.ListCity = _dbContext.Cities.Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            ViewBag.Hotels = _dbContext.Hotels.OrderByDescending(x => x.PublishDate).Where(p => p.Status == HotelStatus.Publish &&
            p.IsSelected == true).Take(6)
                .Select(x => new HotelListDto
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

            ViewBag.Restaurants = _dbContext.Restaurants.OrderByDescending(x => x.PublishDate).Where(p => p.Status == RestaurantStatus.Publish &&
                p.IsSelected == true).Take(6)
                    .Select(x => new RestaurantListDto
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

            ViewBag.Trips = _dbContext.Trips.OrderByDescending(x => x.PublishDate).Where(p => p.TripStatus == TripStatus.Publish &&
                p.IsSelected == true).Take(6)
                    .Select(x => new TripListDto
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

            return View();
        }
    }
}
