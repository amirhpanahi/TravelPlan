using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelPlan.Data;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;
using TravelPlan.Services;
using Microsoft.AspNetCore.Authorization;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Dto.Main.Profile;
using System.Drawing.Printing;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using Microsoft.EntityFrameworkCore;

namespace TravelPlan.Controllers
{
    [Authorize(Roles = "Admin,Special User,Tour Leader User,Normal User")]
    public class ProfileController : Controller
    {
        private readonly DatabaseContext _DbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFileUploadService _fileUpload;
        private readonly EmailService _emailService;
        public ProfileController(DatabaseContext dataBaseContext, UserManager<User> userManager, IFileUploadService fileUploadService, RoleManager<Role> roleManager)
        {
            _DbContext = dataBaseContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileUpload = fileUploadService;
            _emailService = new EmailService();
        }
        #region Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            var UserInfoEdit = new UserInfoEditDto
            {
                Id = userId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            var UserPassEdit = new UserPassEditDto();
            var RetValue = new Tuple<UserInfoEditDto, UserPassEditDto>(UserInfoEdit, UserPassEdit);

            var ErrorListInfo = new List<string>();
            if (TempData["ErrorListInfoDto"] != null)
            {
                foreach (var item in (IEnumerable<string>)TempData["ErrorListInfoDto"])
                {
                    ErrorListInfo.Add(item);
                }
            }

            var ErrorListPass = new List<string>();
            if (TempData["ErrorListPassDto"] != null)
            {
                foreach (var item in (IEnumerable<string>)TempData["ErrorListPassDto"])
                {
                    ErrorListPass.Add(item);
                }
            }

            var SuccessListInfo = TempData["SuccessListInfoDto"];
            var SuccessListPass = TempData["SuccessListPassDto"];

            ViewBag.ErrorListInfo = ErrorListInfo;
            ViewBag.ErrorListPass = ErrorListPass;
            ViewBag.SuccessListInfo = SuccessListInfo;
            ViewBag.SuccessListPass = SuccessListPass;



            return View(RetValue);
        }
        #endregion

        #region EditInfo
        public async Task<IActionResult> EditInfo(UserInfoEditDto model)
        {
            var stringPath = "";
            var FindUser = await _userManager.FindByIdAsync(model.Id);
            var ErrorListInfo = new List<string>();

            if (model.FullName == null)
            {
                ErrorListInfo.Add("فرمت وارد شده نام و نام خانوادگی صحیح نمی باشد");
            }
            if (model.Email == null)
            {
                ErrorListInfo.Add("فرمت وارد شده پست الکترونیک صحیح نمی باشد");
            }
            if (model.PhoneNumber == null || model.PhoneNumber.Length != 11)
            {
                ErrorListInfo.Add("فرمت وارد شده شماره تلفن صحیح نمی باشد");
            }
            if (ModelState.IsValid == false)
            {
                TempData["ErrorListInfoDto"] = ErrorListInfo;
                return RedirectToAction("Index", "Profile");
            }
            if (model.File != null)
            {
                if (model.File.Length > 10000000)
                {
                    ErrorListInfo.Add("حجم عکس باید زیر 10 مگابایت باشد");
                    ViewBag.ErrorListInfoDto = ErrorListInfo;
                    return RedirectToAction("Index", "Profile");
                }
                if (model.File.ContentType == "image/png" || model.File.ContentType == "image/jpg" ||
                    model.File.ContentType == "image/jpeg" || model.File.ContentType == "image/gif")
                {
                    stringPath = await _fileUpload.UploadFileAsync(model.File, model.FullName, "Users");
                }
                else
                {
                    ErrorListInfo.Add("نوع فایل باید به صورت عکس باشد");
                    ViewBag.ErrorListInfoDto = ErrorListInfo;
                    return RedirectToAction("Index", "Profile");
                }
            }


            FindUser.FullName = model.FullName;
            FindUser.Email = model.Email;
            FindUser.PhoneNumber = model.PhoneNumber;
            FindUser.PicAddress = model.File == null ? FindUser.PicAddress : $"Media/Users/{stringPath}";

            var result = await _userManager.UpdateAsync(FindUser);

            if (result.Succeeded)
            {
                TempData["SuccessListInfoDto"] = "اطلاعات شما با موفقیت به روزرسانی شد";
                return Redirect("/Profile/index");
            }

            return RedirectToAction("Index", "Profile");
        }
        #endregion

        #region EditPassword
        public async Task<IActionResult> EditPass(UserPassEditDto model)
        {
            var FindUser = await _userManager.FindByIdAsync(model.Id);
            var ErrorListPass = new List<string>();

            if (model.CurrentPassword == null || model.NewPassword == null || model.ConfirmNewPassword == null)
            {
                ErrorListPass.Add("لطفا تمامی فیلد ها را وارد کنید");
                ModelState.AddModelError("","");
            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ErrorListPass.Add("رمز عبور و تایید یکسان نمی باشد");
                ModelState.AddModelError("", "");
            }
            if (ModelState.IsValid == false)
            {
                TempData["ErrorListPassDto"] = ErrorListPass;
                return RedirectToAction("Index", "Profile");
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(FindUser, FindUser.PasswordHash, model.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                ErrorListPass.Add("رمزعبور فعلی صحیح نمی باشد");
                TempData["ErrorListPassDto"] = ErrorListPass;
                return RedirectToAction("Index", "Profile");
            }
            if (result == PasswordVerificationResult.Success)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(FindUser);
                var Changepassword = await _userManager.ResetPasswordAsync(FindUser, token, model.NewPassword);
                if (Changepassword.Succeeded)
                {
                    TempData["SuccessListPassDto"] = "رمز عبور شما با موفقیت به روزرسانی شد";
                    return Redirect("/Profile/Index");
                }
                else
                {
                    ErrorListPass.Add(" رمز عبور باید حداقل یک حروف بزرگ ('A'-'Z') داشته باشد.");
                    ErrorListPass.Add(" .رمز عبور باید حداقل یک رقم ('0'-'9') داشته باشد");
                    ErrorListPass.Add("  .رمز عبور باید حداقل یک کاراکتر غیر الفبایی داشته باشد");
                    ErrorListPass.Add(".رمز عبور باید حداقل 6 کاراکتر باشد");
                    TempData["ErrorListPassDto"] = ErrorListPass;
                    return RedirectToAction("Index", "Profile");
                }
            }
            return RedirectToAction("Index", "Profile");
        }
        #endregion

        #region TripsEachUser
        public async Task<IActionResult> Trips()
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            ViewBag.ListCity = _DbContext.Cities.Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);

            var ListTrip = _DbContext.Trips.Where(p => p.WriterId == userId && p.TripStatus != TripStatus.Delete).Select(p => new TripListDto
            {
                Id = p.Id,
                Name = p.Name,
                IndexImage = p.IndexImage,
                ImageAlt = p.ImageAlt,
                ImageTitle = p.ImageTitle,
                CountryId = p.CountryId,
                CityId = p.CityId,
                IsSelected = p.IsSelected,
                TripStatus = p.TripStatus,
                TripDuration = p.TripDuration
            }).ToList();

            return View(ListTrip);
        }
        #endregion

        #region HotelsEachUser
        public async Task<IActionResult> Hotels()
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            ViewBag.ListCity = _DbContext.Cities.Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);

            var ListHotels = _DbContext.Hotels.Where(p => p.WriterId == userId && p.Status != HotelStatus.Delete).Select(p => new HotelListDto
            {
                Id = p.Id,
                Name = p.Name,
                IndexImage = p.IndexImage,
                ImageAlt = p.ImageAlt,
                ImageTitle = p.ImageTitle,
                CountryId = p.CountryId,
                CityId = p.CityId,
                IsSelected = p.IsSelected,
                Status = p.Status,
            }).ToList();

            return View(ListHotels);
        }
        #endregion

        #region RestaurantsEachUser
        public async Task<IActionResult> Restaurants()
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            ViewBag.ListCity = _DbContext.Cities.Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);

            var ListRestaurant = _DbContext.Restaurants.Where(p => p.WriterId == userId && p.Status != RestaurantStatus.Delete).Select(p => new RestaurantListDto
            {
                Id = p.Id,
                Name = p.Name,
                IndexImage = p.IndexImage,
                ImageAlt = p.ImageAlt,
                ImageTitle = p.ImageTitle,
                CountryId = p.CountryId,
                CityId = p.CityId,
                IsSelected = p.IsSelected,
                Status = p.Status,
            }).ToList();

            return View(ListRestaurant);
        }
        #endregion

        

    }
}
