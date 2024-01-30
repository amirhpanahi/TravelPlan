using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Main.Account;
using TravelPlan.Models.Entities;
using TravelPlan.Services;

namespace TravelPlan.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly DatabaseContext _DbContext;
		private readonly EmailService _emailService;

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
			RoleManager<Role> roleManager, DatabaseContext DbContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_DbContext = DbContext;
			_emailService = new EmailService();
		}

		[HttpGet]
		public IActionResult management()
		{
			return View();
		}

		#region Register
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto model)
		{
			if (model.FullName == null || model.Email == null || model.PhoneNumber == null || model.Password == null || model.ConfirmPassword == null)
			{
				ModelState.AddModelError("", "لطفا تمامی مقادیر را وارد نمایید");
				return View(model);
			}
			var FindUser = await _userManager.FindByEmailAsync(model.Email);
			var UserPhone = _DbContext.Users.Where(p => p.PhoneNumber == model.PhoneNumber).ToList();
			if (model.Password != model.ConfirmPassword)
			{
				ModelState.AddModelError("", "رمز عبور و تکرار رمز عبور یکی نیست");
			}
			if (FindUser != null)
			{
				ModelState.AddModelError("Email", "این ایمیل قبلا ثبت نام شده است");
			}
			if (model.PhoneNumber.Length != 11)
			{
				ModelState.AddModelError("PhoneNumber", "فرمت وارد شده صحیح نمی باشد");
			}
			if (UserPhone.Count != 0)
			{
				ModelState.AddModelError("PhoneNumber", "این شماره تلفن قبلا ثبت شده");
			}
			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			User NewUser = new User
			{
				FullName = model.FullName,
				Email = model.Email,
				UserName = model.Email,
				PhoneNumber = model.PhoneNumber,
				DateRegister = DateTime.Now,
				DateRegisterPresian = ConvertorDateTime.ToPersian(DateTime.Now),
				LastLoginDate = DateTime.Now,
				LastLoginDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
				PicTitle = model.PicTitle == null ? model.FullName : model.PicTitle,
				PicAlt = model.PicAlt == null ? model.FullName : model.PicAlt
			};

			var result = await _userManager.CreateAsync(NewUser, model.Password);

			string message = "";
			if (result.Succeeded)
			{
				var userForRole = await _userManager.FindByEmailAsync(model.Email);
				await _userManager.AddToRoleAsync(userForRole, "Normal User");


				var UserForId = await _userManager.FindByEmailAsync(model.Email);
				var token = await _userManager.GenerateEmailConfirmationTokenAsync(NewUser);
				string callBackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = UserForId.Id, token = token }, protocol: Request.Scheme);
				string body = $"لطفا برای تایید ایمیل خود بر روی لینک زیر کلیک کنید <br/> <a href={callBackUrl}> Link </a>";
				var sendEmail = _emailService.Execute(NewUser.Email, body, "تایید ایمیل");
				if (sendEmail == true)
				{
					return View("SendEmail");
				}

				return RedirectToAction("login", "account");
			}else
			{
				//foreach (var item in result.Errors.ToList())
				//{
				//	message += item.Description + Environment.NewLine;
				//}

				message = "رمز عبور باید حداقل 6 کاراکتر باشد. رمز عبور باید حداقل یک کاراکتر غیر الفبایی داشته باشد. رمز عبور باید حداقل یک رقم ('0'-'9') داشته باشد. رمز عبور باید حداقل یک حروف بزرگ ('A'-'Z') داشته باشد.";

				ViewBag.Message = message;
			}


			return View();
		}
        #endregion

        public IActionResult AccessDenied()
        {
            return View();
        }


        #region Login
        public IActionResult Login(string returnUrl = "/")
		{
			return View(new LoginDto
			{
				ReturnUrl = returnUrl
			});
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginDto model)
		{
			if (model.Email == null || model.Password == null)
			{
				ModelState.AddModelError("", "لطفا تمامی مقادیر را وارد نمایید");
				return View(model);
			}

			var FindUser = await _userManager.FindByEmailAsync(model.Email);

			if (FindUser == null)
			{
				ModelState.AddModelError("", "پسورد یا نام کاربری اشتباه است");
			}

			if (ModelState.IsValid == false)
			{ 
				return View(model);
			}

			_signInManager.SignOutAsync();
		   
			var resultLogin = await _signInManager.PasswordSignInAsync(FindUser, model.Password, model.IsPersistent, true);
			
			if (resultLogin.Succeeded)
			{
				FindUser.LastLoginDate = DateTime.Now;
				FindUser.LastLoginDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

				var resultUpdate = await _userManager.UpdateAsync(FindUser);

				if (resultUpdate.Succeeded)
					return Redirect(model.ReturnUrl);
				else
					return View(FindUser);
			}
			if (resultLogin.RequiresTwoFactor == true)
			{
				//
			}
			if (resultLogin.IsLockedOut)
			{
				//
			}

			ModelState.AddModelError("", "پسورد یا نام کاربری اشتباه است");

			return View();
		}

		#endregion


		#region ForgetPassword
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordDto model)
		{
			if (model.Email == null)
			{
				ModelState.AddModelError("", "لطفا تمامی مقادیر را وارد نمایید");
				return View();
			}
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier);
			string userIdString;
			if (userId != null)
			{
				userIdString = userId.Value;

			}
			else
			{
				userIdString = "a";
			}

			var userfind = await _userManager.FindByIdAsync(userIdString);
			if (userfind != null)
			{
				if (userfind.Email != model.Email)
				{
					ViewBag.SendEmailFaile = "این آدرس ایمیل متعلق به حساب شما نمی باشد";
					return View(model);
				}
			}


			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				ViewBag.SendEmailFaile = "ممکن است ایمیل وارد شده معتبر نباشد! ";
				return View(model);
			}

			string token = await _userManager.GeneratePasswordResetTokenAsync(user);
			string callBackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, protocol: Request.Scheme);
			string Body = $"برای تنظیم مجدد کلمه عبور بر روی لینک زیر کلیک کنید <br/> <a href={callBackUrl}>لینک ریست کردن رمزعبور</a>";
			var sendEmail = _emailService.Execute(user.Email, Body, "فراموشی رمز عبور");

			if (sendEmail == false)
			{
				ViewBag.SendEmailFaile = "لینک تنظیم مجدد رمز عبور برای شما ایمیل نشد";
				return View();
			}
			ViewBag.SendEmailSucces = "لینک تنظیم مجدد رمز عبور برای شما ایمیل شد";
			return View();
		}

        #endregion

        #region Logout

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
        }

        #endregion
    }
}
