using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
    public class UserInfoAdminLayoutViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _DbContext;
        public UserInfoAdminLayoutViewComponent(UserManager<User> userManager, DatabaseContext dataBaseContext)
        {
            _userManager = userManager;
            _DbContext = dataBaseContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(string Id)
        {
            var userFind = await _userManager.FindByIdAsync(Id);
            //var roleFind = await _DbContext.Roles.Where(p => p.Name == "Admin").FirstAsync();
            //var userRole = await _DbContext.UserRoles.FirstOrDefaultAsync(p => p.UserId == userFind.Id && p.RoleId == roleFind.Id);
            var user = new UserListDto
            {
                Id = userFind.Id,
                FullName = userFind.FullName,
                Email = userFind.Email,
                PhoneNumber = userFind.PhoneNumber,
                EmailConfirmed = userFind.EmailConfirmed,
                PicAddress = userFind.PicAddress,
                PicAlt = userFind.PicAlt,
                PicTitle = userFind.PicTitle,
                //IsAdmin = userRole == null ? false : true
            };
            return View(user);
        }
    }
}
