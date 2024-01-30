using Microsoft.AspNetCore.Mvc;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Main.Search;

namespace TravelPlan.Controllers
{
    public class SearchController : Controller
    {
        [Route("/Search/ByName")]
        public IActionResult SearchByName(string Name)
        {
            var result = new SearchByNameDto
            {
                Name = Name
            };
            return View(result);
        }

        [Route("/Search/City/{Name}")]
        public IActionResult SearchByCity(string Name)
        {
            return View();
        }
    }
}
