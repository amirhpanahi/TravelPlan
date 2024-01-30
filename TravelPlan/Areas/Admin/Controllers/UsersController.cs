using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Dto.Main.Account;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager,
            IFileUploadService fileUpload,DatabaseContext databaseContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        #region Index
        public async Task<IActionResult> Index(string FoundUser)
        {   
            if (FoundUser != null)  
            {
                var listFoundUser = await SearchByFullName(FoundUser);
                if (listFoundUser.Count == 0)
                {
                    ViewBag.NoUserFound = "کاربری با این مشخصات یافت نشد";
                }
                return View(listFoundUser);
            }
            var ListUser = _userManager.Users.OrderByDescending(x => x.DateRegister).Select(p => new UserListDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                EmailConfirmed = p.EmailConfirmed,
                PicAddress = p.PicAddress,
                PicAlt = p.PicAlt,
                PicTitle = p.PicTitle,
                DateRegisterPresian = p.DateRegisterPresian,
                LastLoginDatePersian = p.LastLoginDatePersian,
                DateRegister = p.DateRegister,
                LastLoginDate = p.LastLoginDate
            }).ToList();

            return View(ListUser);
        }
        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterDto registerUser)
        {
            var FindUser = await _userManager.FindByEmailAsync(registerUser.Email);
            if (registerUser.Password != registerUser.ConfirmPassword)
            {
                ModelState.AddModelError("", "رمز عبور و تکرار رمز عبور یکی نیست");
            }
            if (FindUser != null)
            {
                ModelState.AddModelError("Email", "این ایمیل قبلا ثبت نام شده است");
            }
            if (registerUser.PhoneNumber.Length != 11)
            {
                ModelState.AddModelError("PhoneNumber", "فرمت وارد شده صحیح نمی باشد");
            }
            if (ModelState.IsValid == false)
            {
                return View(registerUser);
            }
            User NewUser = new User
            {
                FullName = registerUser.FullName,
                Email = registerUser.Email,
                UserName = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber,
                DateRegister = DateTime.Now,
                DateRegisterPresian = ConvertorDateTime.ToPersian(DateTime.Now),
                LastLoginDate = DateTime.Now,
                LastLoginDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
                PicTitle = registerUser.PicTitle == null ? registerUser.FullName: registerUser.PicTitle,
                PicAlt = registerUser.PicAlt == null ? registerUser.FullName: registerUser.PicAlt
            };

            var result = await _userManager.CreateAsync(NewUser, registerUser.Password);

            string message = "";
            if (result.Succeeded)
            {
                var userForRole = await _userManager.FindByEmailAsync(registerUser.Email);
                await _userManager.AddToRoleAsync(userForRole, "Normal User");
                return RedirectToAction("Index", "users", new { Areas = "admin" });
            }
            else
            {
                message = "رمز عبور باید حداقل 6 کاراکتر باشد. رمز عبور باید حداقل یک کاراکتر غیر الفبایی داشته باشد. رمز عبور باید حداقل یک رقم ('0'-'9') داشته باشد. رمز عبور باید حداقل یک حروف بزرگ ('A'-'Z') داشته باشد.";

                ViewBag.Message = message;
            }


            return View();
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var userEdit = new UserEditDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PicAddress = user.PicAddress,
                PicAlt = user.PicAlt,
                PicTitle = user.PicTitle,
            };
            return View(userEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditDto model)
        {
            var stringPath = "";
            var FindUser = await _userManager.FindByIdAsync(model.Id);
            if (FindUser == null)
            {
                ModelState.AddModelError("Email", "این ایمیل قبلا ثبت نام شده است");
            }
            if (model.PhoneNumber.Length != 11)
            {
                ModelState.AddModelError("PhoneNumber", "فرمت وارد شده صحیح نمی باشد");
            }
            if (model.File != null)
            {
                if (model.File.Length > 1000000)
                {
                    ModelState.AddModelError("PicAddress", "حجم عکس باید زیر یک مگابایت باشد");
                    var userEdit = new UserEditDto()
                    {
                        Id = model.Id,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        PicAddress = FindUser.PicAddress,
                        PicAlt = model.PicAlt,
                        PicTitle = model.PicTitle,
                    };
                    return View(userEdit);
                }
                if (model.File.ContentType == "image/png" || model.File.ContentType == "image/jpg" ||
                    model.File.ContentType == "image/jpeg" || model.File.ContentType == "image/gif")
                {
                    stringPath = await _fileUpload.UploadFileAsync(model.File, model.FullName, "Users");
                }
                else
                {
                    ModelState.AddModelError("PicAddress", "نوع فایل باید به صورت عکس باشد");
                    var userEdit = new UserEditDto()
                    {
                        Id = model.Id,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        PicAddress = FindUser.PicAddress,
                        PicAlt = model.PicAlt,
                        PicTitle = model.PicTitle,
                    };
                    return View(userEdit);
                }
            }
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            FindUser.FullName = model.FullName;
            FindUser.Email = model.Email;
            FindUser.PhoneNumber = model.PhoneNumber;
            FindUser.PicAddress = model.File == null ? FindUser.PicAddress : $"Media/Users/{stringPath}";
            FindUser.PicAlt = model.PicAlt == null ? model.FullName: model.PicAlt;
            FindUser.PicTitle = model.PicTitle == null ? model.FullName: model.PicTitle;

            var result = await _userManager.UpdateAsync(FindUser);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "users", new { Areas = "admin" });
            }

            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }

            ViewBag.Message = message;

            return View();
        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var userDelete = new UserDeleteDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PicAddress = user.PicAddress,
                PicAlt = user.PicAlt,
                PicTitle = user.PicTitle

            };
            return View(userDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDeleteDto model)
        {
            var FindUser = await _userManager.FindByIdAsync(model.Id);
            var result = await _userManager.DeleteAsync(FindUser);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "users", new { Areas = "admin" });
            }

            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }

            ViewBag.Message = message;

            return View();
        }
        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var userDetails = new UserDetailsDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName,
                AccessFailedCount = user.AccessFailedCount,
                DateRegister = Convert.ToString(user.DateRegister),
                LastLoginDate = Convert.ToString(user.LastLoginDate),
                DateRegisterPresian = user.DateRegisterPresian,
                LastLoginDatePersian = user.LastLoginDatePersian,
            };

            return View(userDetails);
        }

        #endregion

        #region AddUserRole

        [HttpGet]
        public async Task<IActionResult> AddUserRole(string Id)
        {
            var userFind = await _userManager.FindByIdAsync(Id);
            var roles = new List<SelectListItem>(
                _roleManager.Roles.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id
                }).ToList());

            return View(new AddUserRoleDto
            {
                Id = Id,
                Roles = roles,
                FullName = userFind.FullName,
                Email = userFind.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(AddUserRoleDto model)
        {
            var userFind = await _userManager.FindByIdAsync(model.Id);
            var roleFind = await _roleManager.FindByIdAsync(model.Role);
            await _userManager.AddToRoleAsync(userFind, roleFind.Name);

            return RedirectToAction("UserRoles", "Users", new { Areas = "admin", id = userFind.Id });
        }



        #endregion

        #region DeleteUserRole

        [HttpGet]
        public async Task<IActionResult> DeleteUserRole(string Id)
        {
            var userFind = await _userManager.FindByIdAsync(Id);
            var ListUserRole = await _userManager.GetRolesAsync(userFind);

            var roles = new List<SelectListItem>();
            foreach (var item in ListUserRole)
            {
                roles.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }

            return View(new AddUserRoleDto
            {
                Id = Id,
                Roles = roles,
                FullName = userFind.FullName,
                Email = userFind.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserRole(AddUserRoleDto model)
        {
            var userFind = await _userManager.FindByIdAsync(model.Id);
            var result = await _userManager.RemoveFromRoleAsync(userFind, model.Role);
            if (result.Succeeded)
            {
                return RedirectToAction("UserRoles", "Users", new { Areas = "admin", id = userFind.Id });
            }
            return View();
        }

        #endregion

        #region UserRoles
        [HttpGet]
        public async Task<IActionResult> UserRoles(string Id)
        {
            var userFind = await _userManager.FindByIdAsync(Id);
            var roles = await _userManager.GetRolesAsync(userFind);

            return View(roles);
        }
        #endregion

        #region SearchByFullName
        public async Task<List<UserListDto>> SearchByFullName(string FullName)
        {
            var userFind = _DbContext.Users.Where(x =>x.FullName.Contains(FullName)).Select(p => new UserListDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                EmailConfirmed = p.EmailConfirmed,
                PicAddress = p.PicAddress,
                PicAlt = p.PicAlt,
                PicTitle = p.PicTitle,
                DateRegisterPresian = p.DateRegisterPresian,
                LastLoginDatePersian = p.LastLoginDatePersian,
                DateRegister = p.DateRegister,
                LastLoginDate = p.LastLoginDate
            }).ToList();

            return userFind;
        }
        #endregion
    }
}
