using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Other;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class NewestHotelViewComponent : ViewComponent
    {
        private readonly DatabaseContext _dbContext;
        public NewestHotelViewComponent(DatabaseContext dataBaseContex)
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

            var Hotels = _dbContext.Hotels.OrderByDescending(x => x.PublishDate).Where(p => p.Status == HotelStatus.Publish).Take(6)
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

            return View(Hotels);
        }
    }
}
