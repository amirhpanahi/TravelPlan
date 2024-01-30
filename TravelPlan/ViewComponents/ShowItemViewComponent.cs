using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Dto.Main.Comment;
using TravelPlan.Models.Dto.Main.ViewComponent;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class ShowItemViewComponent : ViewComponent
    {
        private readonly DatabaseContext _dbContext;
        public ShowItemViewComponent(DatabaseContext dataBaseContex)
        {
            _dbContext = dataBaseContex;
        }
        public async Task<IViewComponentResult> InvokeAsync(string SenderName, string Slug, string UserIdVisitor)
        {

            var FoundHotel = new ShowItemDto();
            var FoundRestaurant = new ShowItemDto();
            var FoundTrip = new ShowItemDto();
            if (SenderName == "Hotel")
            {
                FoundHotel =await _dbContext.Hotels.Where(p => p.Slug == Slug).Select(p=> new ShowItemDto
                {
                    Id = p.Id,
                    Name=p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                    Description = p.Description,
                    AddressAndDetails = p.AddressAndDetails,
                    Summary = p.HotelSummary,
                    Tags = p.Tags,
                    PublishDatePersian = p.PublishDatePersian,
                }).FirstOrDefaultAsync();

            }
            else if (SenderName == "Trip")
            {
                FoundTrip = await _dbContext.Trips.Where(p => p.Slug == Slug).Select(p => new ShowItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                    Description = p.Description,
                    AddressAndDetails = p.AddressAndDetails,
                    Summary = p.TripSummary,
                    Tags = p.Tags,
                    PublishDatePersian = p.PublishDatePersian,
                }).FirstOrDefaultAsync();
            }
            else
            {
                FoundRestaurant = await _dbContext.Restaurants.Where(p => p.Slug == Slug).Select(p => new ShowItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                    Description = p.Description,
                    AddressAndDetails = p.AddressAndDetails,
                    Summary = p.RestaurantSummary,
                    Tags = p.Tags,
                    PublishDatePersian = p.PublishDatePersian,
                }).FirstOrDefaultAsync();
            }

            if (FoundHotel.Name != null)
            {
                var Like = new Like();
                if (UserIdVisitor != null)
                    Like = await _dbContext.Likes.FirstOrDefaultAsync(p => p.ItemId == FoundHotel.Id && p.UserId == UserIdVisitor && p.TypeLike == TypeLike.Hotel);

                var ListComments = _dbContext.Comments.Where(p => p.Status == Comment.StatusName.Publish && p.ItemId == FoundHotel.Id).Select(p => new CommentListDto
                {
                    Id = p.Id,
                    ItemId = p.ItemId,
                    UserId = p.UserId,
                    WriterName = _dbContext.Users.Where(x => x.Id == p.UserId).Select(x => x.FullName).First(),
                    CommentText = p.CommentText,
                    RegisterDate = p.RegisterDate,
                    RegisterDatePersian = p.RegisterDatePersian,
                    ParentId = p.ParentId,
                }).ToList();

                var NumberOfLike = await _dbContext.Likes.Where(p => p.ItemId == FoundHotel.Id && p.StatusLike == StatusLike.Like && p.TypeLike==TypeLike.Hotel).CountAsync();
                var NumberOfComment = await _dbContext.Comments.Where(p => p.ItemId == FoundHotel.Id && p.Status == Comment.StatusName.Publish && p.typeComment == Comment.TypeComment.Hotel).CountAsync();

                var result = new ShowItemDto
                {
                    Name = FoundHotel.Name,
                    IndexImage = FoundHotel.IndexImage,
                    ImageAlt = FoundHotel.ImageAlt,
                    ImageTitle = FoundHotel.ImageTitle,
                    Description = FoundHotel.Description,
                    AddressAndDetails = FoundHotel.AddressAndDetails,
                    Summary = FoundHotel.Summary,
                    Tags = FoundHotel.Tags,
                    PublishDatePersianDay = getDay(FoundHotel.PublishDatePersian),
                    PublishDatePersianMonth = getmonth(FoundHotel.PublishDatePersian),
                    PublishDatePersianYear = getYear(FoundHotel.PublishDatePersian),
                    PublishDatePersianTime = getTime(FoundHotel.PublishDatePersian),
                    CountOfLike = NumberOfLike,
                    CountOfComment = NumberOfComment,
                    LikeStatus = Like == null ? "" : Like.StatusLike.ToString(),
                    Comments = ListComments

                };
                return View(result);
            }
            else if (FoundRestaurant.Name != null)
            {
                var Like = new Like();
                if (UserIdVisitor != null)
                    Like = await _dbContext.Likes.FirstOrDefaultAsync(p => p.ItemId == FoundRestaurant.Id && p.UserId == UserIdVisitor && p.TypeLike == TypeLike.Restaurant);

                var ListComments = _dbContext.Comments.Where(p => p.Status == Comment.StatusName.Publish && p.ItemId == FoundRestaurant.Id).Select(p => new CommentListDto
                {
                    Id = p.Id,
                    ItemId = p.ItemId,
                    UserId = p.UserId,
                    WriterName = _dbContext.Users.Where(x => x.Id == p.UserId).Select(x => x.FullName).First(),
                    CommentText = p.CommentText,
                    RegisterDate = p.RegisterDate,
                    RegisterDatePersian = p.RegisterDatePersian,
                    ParentId = p.ParentId,
                }).ToList();

                var NumberOfLike = await _dbContext.Likes.Where(p => p.ItemId == FoundRestaurant.Id && p.StatusLike == StatusLike.Like && p.TypeLike == TypeLike.Restaurant).CountAsync();
                var NumberOfComment = await _dbContext.Comments.Where(p => p.ItemId == FoundRestaurant.Id && p.Status == Comment.StatusName.Publish && p.typeComment == Comment.TypeComment.Restaurant).CountAsync();

                var result = new ShowItemDto
                {
                    Name = FoundRestaurant.Name,
                    IndexImage = FoundRestaurant.IndexImage,
                    ImageAlt = FoundRestaurant.ImageAlt,
                    ImageTitle = FoundRestaurant.ImageTitle,
                    Description = FoundRestaurant.Description,
                    AddressAndDetails = FoundRestaurant.AddressAndDetails,
                    Summary = FoundRestaurant.Summary,
                    Tags = FoundRestaurant.Tags,
                    PublishDatePersianDay = getDay(FoundRestaurant.PublishDatePersian),
                    PublishDatePersianMonth = getmonth(FoundRestaurant.PublishDatePersian),
                    PublishDatePersianYear = getYear(FoundRestaurant.PublishDatePersian),
                    PublishDatePersianTime = getTime(FoundRestaurant.PublishDatePersian),
                    CountOfLike = NumberOfLike,
                    CountOfComment = NumberOfComment,
                    LikeStatus = Like == null ? "" : Like.StatusLike.ToString(),
                    Comments = ListComments
                };
                return View(result);
            }
            else
            {
                var Like = new Like();
                if (UserIdVisitor != null)
                    Like = await _dbContext.Likes.FirstOrDefaultAsync(p => p.ItemId == FoundTrip.Id && p.UserId == UserIdVisitor && p.TypeLike == TypeLike.Trip);

                var ListComments = _dbContext.Comments.Where(p => p.Status == Comment.StatusName.Publish && p.ItemId == FoundTrip.Id).Select(p => new CommentListDto
                {
                    Id = p.Id,
                    ItemId = p.ItemId,
                    UserId = p.UserId,
                    WriterName = _dbContext.Users.Where(x => x.Id == p.UserId).Select(x => x.FullName).First(),
                    CommentText = p.CommentText,
                    RegisterDate = p.RegisterDate,
                    RegisterDatePersian = p.RegisterDatePersian,
                    ParentId = p.ParentId,
                }).ToList();

                var NumberOfLike = await _dbContext.Likes.Where(p => p.ItemId == FoundTrip.Id && p.StatusLike == StatusLike.Like && p.TypeLike == TypeLike.Trip).CountAsync();
                var NumberOfComment = await _dbContext.Comments.Where(p => p.ItemId == FoundTrip.Id && p.Status == Comment.StatusName.Publish && p.typeComment == Comment.TypeComment.Trip).CountAsync();


                var result = new ShowItemDto
                {
                    Name = FoundTrip.Name,
                    IndexImage = FoundTrip.IndexImage,
                    ImageAlt = FoundTrip.ImageAlt,
                    ImageTitle = FoundTrip.ImageTitle,
                    Description = FoundTrip.Description,
                    AddressAndDetails = FoundTrip.AddressAndDetails,
                    Summary = FoundTrip.Summary,
                    Tags = FoundTrip.Tags,
                    PublishDatePersianDay = getDay(FoundTrip.PublishDatePersian),
                    PublishDatePersianMonth = getmonth(FoundTrip.PublishDatePersian),
                    PublishDatePersianYear = getYear(FoundTrip.PublishDatePersian),
                    PublishDatePersianTime = getTime(FoundTrip.PublishDatePersian),
                    CountOfLike = NumberOfLike,
                    CountOfComment = NumberOfComment,
                    LikeStatus = Like == null ? "" : Like.StatusLike.ToString(),
                    Comments = ListComments
                };
                return View(result);
            }
        }


        private string getDay(string date)
        {
            var SeprateDayMonth = date.Split(" ");
            var GetDayMonth = SeprateDayMonth[0].Split("/");
            return GetDayMonth[2];
        }
        private string getmonth(string date)
        {
            var SeprateDayMonth = date.Split(" ");
            var GetDayMonth = SeprateDayMonth[0].Split("/");
            var month = GetDayMonth[1];
            var Retmonth = "";
            switch (month)
            {
                case "1":
                    Retmonth = "فروردین";
                    break;
                case "2":
                    Retmonth = "اردیبهشت";
                    break;
                case "3":
                    Retmonth = "خرداد";
                    break;
                case "4":
                    Retmonth = "تیر";
                    break;
                case "5":
                    Retmonth = "مرداد";
                    break;
                case "6":
                    Retmonth = "شهریور";
                    break;
                case "7":
                    Retmonth = "مهر";
                    break;
                case "8":
                    Retmonth = "آبان";
                    break;
                case "9":
                    Retmonth = "آذر";
                    break;
                case "10":
                    Retmonth = "دی";
                    break;
                case "11":
                    Retmonth = "بهمن";
                    break;
                case "12":
                    Retmonth = "اسفند";
                    break;
            }
            return Retmonth;
        }
        private string getYear(string date)
        {
            var SeprateDayMonth = date.Split(" ");
            var GetDayMonth = SeprateDayMonth[0].Split("/");
            return GetDayMonth[0];
        }
        private string getTime(string date)
        {
            var SeprateDayMonth = date.Split(" ");
            var GetDayTime = SeprateDayMonth[1].Split(":");
            return GetDayTime[0] + ":" + GetDayTime[1];
        }
    }
}
