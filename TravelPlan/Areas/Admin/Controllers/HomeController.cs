using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Banner;
using TravelPlan.Models.Dto.Area.Home;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;
using static TravelPlan.Models.Entities.Comment;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class HomeController : Controller
	{
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public HomeController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }


        [Route("/Admin/Index")]
        public async Task<IActionResult> Index()
		{
            var allTrip = await _DbContext.Trips.CountAsync();
            var PublishTrip = await _DbContext.Trips.Where(p => p.TripStatus == TripStatus.Publish).CountAsync();
            var RejectTrip = await _DbContext.Trips.Where(p => p.TripStatus == TripStatus.RejectedByAdmin).CountAsync();
            var WatingTrip = await _DbContext.Trips.Where(p => p.TripStatus == TripStatus.WaitingForConfirm).CountAsync();
            var PercentOfWatitTrip = WatingTrip == 0 ? 0 : (int)Math.Floor((decimal)(WatingTrip * 100) / allTrip);

            var allHotel = await _DbContext.Hotels.CountAsync();
            var PublishHotel = await _DbContext.Hotels.Where(p => p.Status == HotelStatus.Publish).CountAsync();
            var RejectHotel = await _DbContext.Hotels.Where(p => p.Status == HotelStatus.RejectedByAdmin).CountAsync();
            var WatingHotel = await _DbContext.Hotels.Where(p => p.Status == HotelStatus.WaitingForConfirm).CountAsync();
            var PercentOfWatitHotel = WatingHotel == 0 ? 0 : (int)Math.Floor((decimal)(WatingHotel * 100) / allHotel);

            var allRestaurant = await _DbContext.Restaurants.CountAsync();
            var PublishRestaurant = await _DbContext.Restaurants.Where(p => p.Status == RestaurantStatus.Publish).CountAsync();
            var RejectRestaurant = await _DbContext.Restaurants.Where(p => p.Status == RestaurantStatus.RejectedByAdmin).CountAsync();
            var WatingRestaurant = await _DbContext.Restaurants.Where(p => p.Status == RestaurantStatus.WaitingForConfirm).CountAsync();
            var PercentOfWatitRestaurant = WatingRestaurant == 0 ? 0 : (int)Math.Floor((decimal)(WatingRestaurant * 100) / allRestaurant);

            var allComment = await _DbContext.Comments.CountAsync();
            var PublishComment = await _DbContext.Comments.Where(p => p.Status == StatusName.Publish).CountAsync();
            var RejectComment = await _DbContext.Comments.Where(p => p.Status == StatusName.RejectedByAdmin).CountAsync();
            var WatingComment = await _DbContext.Comments.Where(p => p.Status == StatusName.WaitingForConfirm).CountAsync();
            var PercentOfWatitComment = WatingComment == 0 ? 0 : (int)Math.Floor((decimal)(WatingComment * 100) / allComment);



            var home = new AdminIndexPageDto
            {
                allHotel = allHotel,
                allComment = allComment,
                allRestaurant = allRestaurant,
                allTrip = allTrip,

                PublishComment = PublishComment,
                PublishHotel = PublishHotel,
                PublishRestaurant = PublishRestaurant,
                PublishTrip = PublishTrip,

                RejectComment = RejectComment,
                RejectHotel = RejectHotel,
                RejectRestaurant = RejectRestaurant,
                RejectTrip = RejectTrip,

                WatingComment = WatingComment,
                WatingHotel = WatingHotel,
                WatingRestaurant = WatingRestaurant,
                WatingTrip = WatingTrip,

                PercentOfWatitingRestaurant = PercentOfWatitRestaurant,
                PercentOfWatitingComment = PercentOfWatitComment,
                PercentOfWatitingHotel = PercentOfWatitHotel,
                PercentOfWatitingTrip = PercentOfWatitTrip,
            };
            return View(home);
        }
    }
}
