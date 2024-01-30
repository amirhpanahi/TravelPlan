using Microsoft.AspNetCore.Mvc;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class NewestTripViewComponent : ViewComponent
    {
        private readonly DatabaseContext _dbContext;
        public NewestTripViewComponent(DatabaseContext dataBaseContex)
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

            var Trips = _dbContext.Trips.OrderByDescending(x => x.PublishDate).Where(p => p.TripStatus == TripStatus.Publish).Take(6)
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

            return View(Trips);
        }
    }
}
