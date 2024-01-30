using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Banner;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public BannerController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region MainBanners
        [Route("/Admin/MainBanners")]
        [HttpGet]
        public IActionResult MainBanners()
        {
            var baner1 = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP1");
            var baner2 = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP2");
            var baner3 = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP3");
            var baner4 = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP4");
            var baner5 = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP5");

            var banner = new BannersDto
            {
                BannerAddress1 = baner1 == null ? "" : baner1.BannerAddress,
                BannerPhotoText1 = baner1 == null ? "" : baner1.PhotoText,
                BannerPhotoLink1 = baner1 == null ? "" : baner1.PhotoLink,
                BannerAlt1 = baner1 == null ? "" : baner1.BannerAlt,
                BannerTitle1 = baner1 == null ? "" : baner1.BannerTitle,

                BannerAddress2 = baner2 == null ? "" : baner2.BannerAddress,
                BannerPhotoText2 = baner2 == null ? "" : baner2.PhotoText,
                BannerPhotoLink2 = baner2 == null ? "" : baner2.PhotoLink,
                BannerAlt2 = baner2 == null ? "" : baner2.BannerAlt,
                BannerTitle2 = baner2 == null ? "" : baner2.BannerTitle,

                BannerAddress3 = baner3 == null ? "" : baner3.BannerAddress,
                BannerPhotoText3 = baner3 == null ? "" : baner3.PhotoText,
                BannerPhotoLink3 = baner3 == null ? "" : baner3.PhotoLink,
                BannerAlt3 = baner3 == null ? "" : baner3.BannerAlt,
                BannerTitle3 = baner3 == null ? "" : baner3.BannerTitle,

                BannerAddress4 = baner4 == null ? "" : baner4.BannerAddress,
                BannerPhotoText4 = baner4 == null ? "" : baner4.PhotoText,
                BannerPhotoLink4 = baner4 == null ? "" : baner4.PhotoLink,
                BannerAlt4 = baner4 == null ? "" : baner4.BannerAlt,
                BannerTitle4 = baner4 == null ? "" : baner4.BannerTitle,

                BannerAddress5 = baner5 == null ? "" : baner5.BannerAddress,
                BannerPhotoText5 = baner5 == null ? "" : baner5.PhotoText,
                BannerPhotoLink5 = baner5 == null ? "" : baner5.PhotoLink,
                BannerAlt5 = baner5 == null ? "" : baner5.BannerAlt,
                BannerTitle5 = baner5 == null ? "" : baner5.BannerTitle,
            };
            return View(banner);
        }

        [Route("/Admin/MainBanners")]
        [HttpPost]
        public async Task<IActionResult> MainBanners(BannersDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.Banner1 != null)
                {
                    var findGif = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP1");
                    if (findGif == null)
                    {
                        await SaveBanner(model.Banner1, "B-MP1");
                    }
                    else
                    {
                        await EditBanner(model.Banner1, "B-MP1");
                    }
                }
                if (model.BannerAlt1 != null || model.BannerTitle1 != null || model.BannerPhotoLink1 != null || model.BannerPhotoText1 != null)
                {
                    await EditBannerDetails("B-MP1", model.BannerAlt1, model.BannerTitle1, model.BannerPhotoLink1, model.BannerPhotoText1);
                }
                if (model.Banner2 != null)
                {
                    var findGif = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP2");
                    if (findGif == null)
                    {
                        await SaveBanner(model.Banner2, "B-MP2");
                    }
                    else
                    {
                        await EditBanner(model.Banner2, "B-MP2");
                    }
                }
                if (model.BannerAlt2 != null || model.BannerTitle2 != null || model.BannerPhotoLink2 != null || model.BannerPhotoText2 != null)
                {
                    await EditBannerDetails("B-MP2", model.BannerAlt2, model.BannerTitle2, model.BannerPhotoLink2, model.BannerPhotoText2);
                }
                if (model.Banner3 != null)
                {
                    var findGif = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP3");
                    if (findGif == null)
                    {
                        await SaveBanner(model.Banner3, "B-MP3");
                    }
                    else
                    {
                        await EditBanner(model.Banner3, "B-MP3");
                    }
                }
                if (model.BannerAlt3 != null || model.BannerTitle3 != null || model.BannerPhotoLink3 != null || model.BannerPhotoText3 != null)
                {
                    await EditBannerDetails("B-MP3", model.BannerAlt3, model.BannerTitle3, model.BannerPhotoLink3, model.BannerPhotoText3);
                }
                if (model.Banner4 != null)
                {
                    var findGif = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP4");
                    if (findGif == null) 
                    {
                        await SaveBanner(model.Banner4, "B-MP4");
                    }
                    else
                    {
                        await EditBanner(model.Banner4, "B-MP4");
                    }
                }
                if (model.BannerAlt4 != null || model.BannerTitle4 != null || model.BannerPhotoLink4 != null || model.BannerPhotoText4 != null)
                {
                    await EditBannerDetails("B-MP4", model.BannerAlt4, model.BannerTitle4, model.BannerPhotoLink4, model.BannerPhotoText4);
                }
                if (model.Banner5 != null)
                {
                    var findGif = _DbContext.Banners.FirstOrDefault(p => p.BannerName == "B-MP5");
                    if (findGif == null)
                    {
                        await SaveBanner(model.Banner5, "B-MP5");
                    }
                    else
                    {
                        await EditBanner(model.Banner5, "B-MP5");
                    }
                }
                if (model.BannerAlt5 != null || model.BannerTitle5 != null || model.BannerPhotoLink5 != null || model.BannerPhotoText5 != null)
                {
                    await EditBannerDetails("B-MP5", model.BannerAlt5, model.BannerTitle5, model.BannerPhotoLink5, model.BannerPhotoText5);
                }

                return Redirect("/Admin/MainBanners");
            }
            return View();
        }
        #endregion


        public async Task<bool> SaveBanner(IFormFile file, string Name)
        {
            var stringLogoPath = $"Media/Banner/" + await _fileUpload.UploadFileAsync(file, Name, "Banner");
            var Banner = new Banner
            {
                BannerName = Name,
                BannerAddress = stringLogoPath,
            };

            await _DbContext.Banners.AddAsync(Banner);
            await _DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditBanner(IFormFile file, string Name)
        {
            var findBanner = _DbContext.Banners.FirstOrDefault(p => p.BannerName == Name);
            var stringLogoPath = "";

            if (findBanner != null)
                stringLogoPath = $"Media/Banner/" + await _fileUpload.UploadFileAsync(file, Name, "Banner");

            findBanner.BannerAddress = stringLogoPath;

            _DbContext.Entry(findBanner).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditBannerDetails(string Name, string Alt, string Title, string BannerPhotoLink,string BannerPhotoText)
        {
            var findBanner = _DbContext.Banners.FirstOrDefault(p => p.BannerName == Name);

            findBanner.BannerAlt = Alt;
            findBanner.BannerTitle = Title;
            findBanner.PhotoText = BannerPhotoText;
            findBanner.PhotoLink = BannerPhotoLink;

            _DbContext.Entry(findBanner).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return true;
        }
    }
}
