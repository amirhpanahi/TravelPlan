using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Data;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;
using TravelPlan.Services;
using Microsoft.IdentityModel.Tokens;
using TravelPlan.Models.Dto.Main.Comment;
using static TravelPlan.Models.Entities.Comment;
using TravelPlan.Common;

namespace TravelPlan.Controllers
{
    public class ServicesController : Controller
    {
        private readonly DatabaseContext _DbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFileUploadService _fileUpload;
        private readonly EmailService _emailService;
        public ServicesController(DatabaseContext dataBaseContext, UserManager<User> userManager, IFileUploadService fileUploadService, RoleManager<Role> roleManager)
        {
            _DbContext = dataBaseContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileUpload = fileUploadService;
            _emailService = new EmailService();
        }
        public IActionResult Index()
        {
            return View();
        }

        #region LikeItemsAjax
        [HttpPost]
        [Route("Api/Like")]
        public async Task<string> LikeItem(int ItemId, int CountOfLike,string TblName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var TableName = new TypeLike();
            if (TblName == "Trip")
            {
                TableName = TypeLike.Trip;
            }
            else if (TblName == "Hotel")
            {
                TableName = TypeLike.Hotel;
            }
            else
            {
                TableName = TypeLike.Restaurant;
            }
            var FindLike = await _DbContext.Likes.FirstOrDefaultAsync(p => p.ItemId == ItemId && p.UserId == userId && p.TypeLike == TableName);

            if (FindLike != null)
            {
                if (FindLike.StatusLike == StatusLike.Like)
                {
                    FindLike.StatusLike = StatusLike.None;
                    _DbContext.Entry(FindLike).State = EntityState.Modified;
                    await _DbContext.SaveChangesAsync();
                    return $"fa fa-heart-o text-danger mx-2,{CountOfLike -= 1}";
                }
                else
                {
                    FindLike.StatusLike = StatusLike.Like;
                    _DbContext.Entry(FindLike).State = EntityState.Modified;
                    await _DbContext.SaveChangesAsync();
                    return $"fa fa-heart text-danger mx-2,{CountOfLike + 1}";
                }
            }
            else
            {
                var AddLike = new Like
                {
                    ItemId = ItemId,
                    UserId = userId,
                    StatusLike = StatusLike.Like,
                    TypeLike = TableName
                };
                await _DbContext.Likes.AddAsync(AddLike);
                await _DbContext.SaveChangesAsync();
                return $"fa fa-heart text-danger mx-2,{CountOfLike + 1}";
            }
        }
        #endregion

        #region submitComment
        [HttpPost]
        [Route("api/submitComment")]
        public async Task<string> SubmitComment(string IdCommentWriter, string TextComment, int ItemId, string TblName)
        {
            if (TextComment == null || TextComment.Trim().Length == 0 || TblName==null)
            {
                return "error";
            }
            if (TextComment.Length > 1000)
            {
                return "error";
            }

            var FindItem = new CommentDto();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var TableName = new TypeComment();
            if (TblName == "Trip")
            {
                TableName = TypeComment.Trip;
                FindItem = _DbContext.Trips.Where(p => p.Id == ItemId).Select(p => new CommentDto
                {
                    Id = p.Id,
                }).FirstOrDefault();
            }
            else if (TblName == "Hotel")
            {
                TableName = TypeComment.Hotel;
                FindItem = _DbContext.Hotels.Where(p => p.Id == ItemId).Select(p=> new CommentDto
                {
                   Id = p.Id,
                }).FirstOrDefault();
            }
            else
            {
                TableName = TypeComment.Restaurant;
                FindItem = _DbContext.Restaurants.Where(p => p.Id == ItemId).Select(p => new CommentDto
                {
                    Id = p.Id,
                }).FirstOrDefault();
            }


            if (ModelState.IsValid)
            {
                var NewComment = new Comment
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    ItemId = FindItem.Id,
                    CommentText = TextComment,
                    RegisterDate = DateTime.Now,
                    RegisterDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
                    ParentId = 0,
                    Status = Comment.StatusName.WaitingForConfirm,
                    typeComment = TableName,
                    
                };
                await _DbContext.Comments.AddAsync(NewComment);
                await _DbContext.SaveChangesAsync();
                return "دیدگاه شما ثبت شد لطفا منتظر تایید آن بمانید";
            }
            return "UnKnow";
        }
        #endregion
    }
}
