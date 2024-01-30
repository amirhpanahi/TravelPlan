using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Ads;

namespace TravelPlan.ViewComponents
{
    public class GifsInPageViewComponent : ViewComponent
    {
        private readonly DatabaseContext _dbContext;
        public GifsInPageViewComponent(DatabaseContext dataBaseContex)
        {
            _dbContext = dataBaseContex;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //gif-productPage1
            var Gif1 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP1");
            var Gif2 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP2");
            var Gif3 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP3");
            var Gif4 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP4");
            var Gif5 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP5");
            var Gif6 = _dbContext.Ads.FirstOrDefault(p => p.GifName == "G-PP6");

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
    }
}
