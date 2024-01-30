using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelPlan.Data;
using TravelPlan.Models;
using TravelPlan.Models.Dto.Area.Banner;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public HomeController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        public async Task<IActionResult> Index()
        {

            var Banner1 = await _DbContext.Banners.FirstOrDefaultAsync(x => x.BannerName == "B-MP1");
            var Banner2 = await _DbContext.Banners.FirstOrDefaultAsync(x => x.BannerName == "B-MP2");
            var Banner3 = await _DbContext.Banners.FirstOrDefaultAsync(x => x.BannerName == "B-MP3");
            var Banner4 = await _DbContext.Banners.FirstOrDefaultAsync(x => x.BannerName == "B-MP4");
            var Banner5 = await _DbContext.Banners.FirstOrDefaultAsync(x => x.BannerName == "B-MP5");

            ViewBag.ListBanners = new BannersDto
            {
                BannerAddress1 = Banner1 == null ? "" : Banner1.BannerAddress,
                BannerAddress2 = Banner2 == null ? "" : Banner2.BannerAddress,
                BannerAddress3 = Banner3 == null ? "" : Banner3.BannerAddress,
                BannerAddress4 = Banner4 == null ? "" : Banner4.BannerAddress,
                BannerAddress5 = Banner5 == null ? "" : Banner5.BannerAddress,

                BannerAlt1 = Banner1 == null ? "" : Banner1.BannerAlt,
                BannerAlt2 = Banner2 == null ? "" : Banner2.BannerAlt,
                BannerAlt3 = Banner3 == null ? "" : Banner3.BannerAlt,
                BannerAlt4 = Banner4 == null ? "" : Banner4.BannerAlt,
                BannerAlt5 = Banner5 == null ? "" : Banner5.BannerAlt,

                BannerTitle1 = Banner1 == null ? "" : Banner1.BannerTitle,
                BannerTitle2 = Banner2 == null ? "" : Banner2.BannerTitle,
                BannerTitle3 = Banner3 == null ? "" : Banner3.BannerTitle,
                BannerTitle4 = Banner4 == null ? "" : Banner4.BannerTitle,
                BannerTitle5 = Banner5 == null ? "" : Banner5.BannerTitle,

                BannerPhotoLink1 = Banner1 == null ? "" : Banner1.PhotoLink,
                BannerPhotoLink2 = Banner2 == null ? "" : Banner2.PhotoLink,
                BannerPhotoLink3 = Banner3 == null ? "" : Banner3.PhotoLink,
                BannerPhotoLink4 = Banner4 == null ? "" : Banner4.PhotoLink,
                BannerPhotoLink5 = Banner5 == null ? "" : Banner5.PhotoLink,

                BannerPhotoText1 = Banner1 == null ? "" : Banner1.PhotoText,
                BannerPhotoText2 = Banner2 == null ? "" : Banner2.PhotoText,
                BannerPhotoText3 = Banner3 == null ? "" : Banner3.PhotoText,
                BannerPhotoText4 = Banner4 == null ? "" : Banner4.PhotoText,
                BannerPhotoText5 = Banner5 == null ? "" : Banner5.PhotoText,
            };

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}