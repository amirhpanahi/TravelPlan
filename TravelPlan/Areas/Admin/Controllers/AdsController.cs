using Microsoft.AspNetCore.Mvc;
using TravelPlan.Data;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Models.Dto.Area.Ads;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdsController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public AdsController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }
        public IActionResult Index()
        {
            return View();
        }



        #region GifProductPage
        [HttpGet]
        public IActionResult GifProductPage()
        {
            //gif-productPage1
            var Gif1 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP1");
            var Gif2 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP2");
            var Gif3 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP3");
            var Gif4 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP4");
            var Gif5 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP5");
            var Gif6 = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP6");

            var Ads = new AdsDto
            {
                GifAddress1 = Gif1 == null ? "" : Gif1.GifAddress,
                GifPhotoLink1 = Gif1 == null ? "" : Gif1.PhotoLink,
                GifAlt1 = Gif1 == null ? "" : Gif1.GifAlt,
                GifTitle1 = Gif1 == null ? "" : Gif1.GifTitle,

                GifAddress2 = Gif2 == null ? "" : Gif2.GifAddress,
                GifPhotoLink2 = Gif2 == null ? "" : Gif2.PhotoLink,
                GifAlt2 = Gif2 == null ? "" : Gif2.GifAlt,
                GifTitle2 = Gif2 == null ? "" : Gif2.GifTitle,

                GifAddress3 = Gif3 == null ? "" : Gif3.GifAddress,
                GifPhotoLink3 = Gif3 == null ? "" : Gif3.PhotoLink,
                GifAlt3 = Gif3 == null ? "" : Gif3.GifAlt,
                GifTitle3 = Gif3 == null ? "" : Gif3.GifTitle,

                GifAddress4 = Gif4 == null ? "" : Gif4.GifAddress,
                GifPhotoLink4 = Gif4 == null ? "" : Gif4.PhotoLink,
                GifAlt4 = Gif4 == null ? "" : Gif4.GifAlt,
                GifTitle4 = Gif4 == null ? "" : Gif4.GifTitle,

                GifAddress5 = Gif5 == null ? "" : Gif5.GifAddress,
                GifPhotoLink5 = Gif5 == null ? "" : Gif5.PhotoLink,
                GifAlt5 = Gif5 == null ? "" : Gif5.GifAlt,
                GifTitle5 = Gif5 == null ? "" : Gif5.GifTitle,

                GifAddress6 = Gif6 == null ? "" : Gif6.GifAddress,
                GifPhotoLink6 = Gif6 == null ? "" : Gif6.PhotoLink,
                GifAlt6 = Gif6 == null ? "" : Gif6.GifAlt,
                GifTitle6 = Gif6 == null ? "" : Gif6.GifTitle,
            };
            return View(Ads);
        }

        [HttpPost]
        public async Task<IActionResult> GifProductPage(AdsDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.Gif1 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP1");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif1, "G-PP1");
                    }
                    else
                    {
                        await EditGif(model.Gif1, "G-PP1");
                    }
                }
                if (model.GifAlt1 != null || model.GifTitle1 != null || model.GifPhotoLink1 != null )
                {
                    await EditGifDetails("G-PP1", model.GifAlt1, model.GifTitle1, model.GifPhotoLink1);
                }
                if (model.Gif2 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP2");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif2, "G-PP2");
                    }
                    else
                    {
                        await EditGif(model.Gif2, "G-PP2");
                    }
                }
                if (model.GifAlt2 != null || model.GifTitle2 != null || model.GifPhotoLink2 != null)
                {
                    await EditGifDetails("G-PP2", model.GifAlt2, model.GifTitle2, model.GifPhotoLink2);
                }
                if (model.Gif3 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP3");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif3, "G-PP3");
                    }
                    else
                    {
                        await EditGif(model.Gif3, "G-PP3");
                    }
                }
                if (model.GifAlt3 != null || model.GifTitle3 != null || model.GifPhotoLink3 != null)
                {
                    await EditGifDetails("G-PP3", model.GifAlt3, model.GifTitle3, model.GifPhotoLink3);
                }
                if (model.Gif4 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP4");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif4, "G-PP4");
                    }
                    else
                    {
                        await EditGif(model.Gif4, "G-PP4");
                    }
                }
                if (model.GifAlt4 != null || model.GifTitle4 != null || model.GifPhotoLink4 != null)
                {
                    await EditGifDetails("G-PP4", model.GifAlt4, model.GifTitle4, model.GifPhotoLink4);
                }
                if (model.Gif5 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP5");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif5, "G-PP5");
                    }
                    else
                    {
                        await EditGif(model.Gif5, "G-PP5");
                    }
                }
                if (model.GifAlt5 != null || model.GifTitle5 != null || model.GifPhotoLink5 != null)
                {
                    await EditGifDetails("G-PP5", model.GifAlt5, model.GifTitle5, model.GifPhotoLink5);
                }
                if (model.Gif6 != null)
                {
                    var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP6");
                    if (findGif == null)
                    {
                        await SaveGif(model.Gif6, "G-PP6");
                    }
                    else
                    {
                        await EditGif(model.Gif6, "G-PP6");
                    }
                }
                if (model.GifAlt6 != null || model.GifTitle6 != null || model.GifPhotoLink6 != null)
                {
                    await EditGifDetails("G-PP6", model.GifAlt6, model.GifTitle6, model.GifPhotoLink6);
                }

                return Redirect("/Admin/Ads/GifProductPage");
            }
            return View();
        }
        #endregion


        public async Task<bool> SaveGif(IFormFile file, string Name)
        {
            var stringLogoPath = $"Media/Ads/" + await _fileUpload.UploadFileAsync(file, Name, "Ads");
            var Gif = new Ads
            {
                GifName = Name,
                GifAddress = stringLogoPath,
            };

            await _DbContext.Ads.AddAsync(Gif);
            await _DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditGif(IFormFile file, string Name)
        {
            var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == Name);
            var stringLogoPath = "";

            if (findGif != null)
                stringLogoPath = $"Media/Ads/" + await _fileUpload.UploadFileAsync(file, Name, "Ads");

            findGif.GifAddress = stringLogoPath;

            _DbContext.Entry(findGif).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditGifDetails(string Name, string Alt, string Title, string GifPhotoLink)
        {
            var findGif = _DbContext.Ads.FirstOrDefault(p => p.GifName == Name);

            findGif.GifAlt = Alt;
            findGif.GifTitle = Title;
            findGif.PhotoLink = GifPhotoLink;

            _DbContext.Entry(findGif).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return true;
        }

    }
}
