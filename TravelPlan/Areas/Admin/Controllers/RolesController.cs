using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelPlan.Models.Dto.Area.Roles;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Entities;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var Roles = _roleManager.Roles.OrderBy(x=>x.Name).Select(p => new RoleListDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
            }).ToList();
            return View(Roles);
        }


        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateDto roleList)
        {
            var FindUser = await _roleManager.FindByNameAsync(roleList.Name);
            if (FindUser != null)
            {
                ModelState.AddModelError("Name", "این نقش قبلا ثبت  شده است");
            }
            if (ModelState.IsValid == false)
            {
                return View(roleList);
            }
            Role NewRole = new Role
            {
                Name = roleList.Name,
                Description = roleList.Description
            };

            var result = await _roleManager.CreateAsync(NewRole);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "roles", new { Areas = "admin" });
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

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            var roleEdit = new RoleListDto()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            return View(roleEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleListDto model)
        {
            var FindRole = await _roleManager.FindByIdAsync(model.Id);
            if (FindRole == null)
            {
                ModelState.AddModelError("Email", "این ایمیل قبلا ثبت نام شده است");
            }
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            FindRole.Name = model.Name;
            FindRole.Description = model.Description;

            var result = await _roleManager.UpdateAsync(FindRole);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "roles", new { Areas = "admin" });
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
            var role = await _roleManager.FindByIdAsync(Id);
            var roleDelete = new RoleListDto()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            return View(roleDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RoleListDto model)
        {
            var FindRole = await _roleManager.FindByIdAsync(model.Id);
            var result = await _roleManager.DeleteAsync(FindRole);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "roles", new { Areas = "admin" });
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

        #region Users In Role
        [HttpGet]
        public async Task<IActionResult> UsersInRole(string Name)
        {
            var ListUser = await _userManager.GetUsersInRoleAsync(Name);

            var users = ListUser.OrderByDescending(x => x.DateRegister).Select(p => new UserListDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                EmailConfirmed = p.EmailConfirmed,
                DateRegisterPresian = p.DateRegisterPresian,
                LastLoginDatePersian = p.LastLoginDatePersian,
                PicAddress = p.PicAddress,
                PicTitle = p.PicTitle,
                PicAlt = p.PicAlt,
            }).ToList();

            ViewBag.roleName = Name;

            return View(users);
        }
        #endregion
    }
}
