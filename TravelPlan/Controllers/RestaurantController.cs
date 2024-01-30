using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Dto.Main.ViewComponent;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Controllers
{
    [Authorize(Roles = "Admin,Special User,Tour Leader User,Normal User")]
    public class RestaurantController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public RestaurantController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ShowRestaurant
        [HttpGet]
        [Route("Restaurant/{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdVisitor = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ViewBag.userIdVisitor = userIdVisitor;
            }
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Slug == slug);
            if (findRestaurant == null)
            {
                return Redirect("/NotFound");
            }
            var model = new ShowItemDto
            {
                Id = findRestaurant.Id,
                Slug = slug,
                //CountOfLike = await _DbContext.Likes.Where(p => p.NewsId == findNews.Id && p.StatusLike == StatusLike.Like).CountAsync()
            };
            return View(model);

        }
        #endregion

        #region Create
        [HttpGet]
        [Route("/Profile/Restaurant/Create")]
        public IActionResult Create()
        {
            ViewBag.ListCountrySelectList = new List<SelectListItem>(
                _DbContext.Countries.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            return View();
        }


        [HttpPost]
        [Route("/Profile/Restaurant/Create")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Create(RestaurantCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Restaurants");
            }

            var Countries = new List<SelectListItem>(
                        _DbContext.Countries.Select(p => new SelectListItem
                        {
                            Text = p.Name,
                            Value = p.Id.ToString()
                        }).ToList());

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var stringImagePath = "";
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 5242848)
                {
                    ModelState.AddModelError("ImageFile", "حجم عکس باید زیر پنج مگابایت باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    return View(model);
                }
                if (model.ImageFile.ContentType == "image/png" || model.ImageFile.ContentType == "image/jpg" ||
                    model.ImageFile.ContentType == "image/jpeg" || model.ImageFile.ContentType == "image/gif")
                {
                    stringImagePath = $"Media/Restaurant/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Restaurant");
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "نوع فایل باید به صورت عکس باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    return View(model);
                }
            }
            else
                stringImagePath = null;

            var stringVideoPath = "";
            if (model.VideoFile != null)
            {
                if (model.VideoFile.Length > 104856975)
                {
                    ModelState.AddModelError("VideoFile", "حجم ویدیو باید زیر 100 مگابایت باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    return View(model);
                }
                if (model.VideoFile.ContentType == "video/mp4" || model.VideoFile.ContentType == "video/wmv")
                {
                    stringVideoPath = $"Media/Restaurant/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Restaurant", "Video");
                }
                else
                {
                    ModelState.AddModelError("VideoFile", "نوع فایل باید به صورت ویدیو باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    return View(model);
                }
            }
            else
                stringVideoPath = null;


            if (ModelState.IsValid)
            {
                var NewRestaurant = new Restaurant
                {
                    Name = model.Name,
                    Slug = repaceSlug,
                    Description = model.Description,
                    KeyWords = model.KeyWords == null ? null : model.KeyWords,
                    VideoAddress = stringVideoPath,
                    IndexImage = stringImagePath,
                    ImageAlt = model.ImageAlt != null ? model.ImageAlt : model.Name,
                    ImageTitle = model.ImageTitle != null ? model.ImageTitle : model.Name,
                    RegisterDate = DateTime.Now.ToString(),
                    RegisterDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
                    WriterId = userId,
                    IsSelected = model.IsSelected,
                    Status = RestaurantStatus.WaitingForConfirm,
                    Tags = model.Tags == null ? null : model.Tags,
                    RestaurantSummary = model.RestaurantSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                };

                await _DbContext.Restaurants.AddAsync(NewRestaurant);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Restaurants");
            }
            return View(model);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Route("/Restaurant/Edit/{Id}")]
        public IActionResult Edit(int Id)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Id == Id);
            if (findRestaurant.Status == RestaurantStatus.Publish)
            { }
            else
            {
                return Redirect("/Profile/Restaurants");
            }
            ViewBag.ListCountrySelectList = new List<SelectListItem>(
            _DbContext.Countries.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList());
            ViewBag.ListFirstCities = new List<SelectListItem>(
                _DbContext.Cities.Where(x => x.CountryId == findRestaurant.CountryId).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            return View(new RestaurantEditDto
            {
                Id = findRestaurant.Id,
                Name = findRestaurant.Name,
                Slug = findRestaurant.Slug,
                Description = findRestaurant.Description,
                AddressAndDetails = findRestaurant.AddressAndDetails,
                RestaurantSummary = findRestaurant.RestaurantSummary,
                Tags = findRestaurant.Tags,
                KeyWords = findRestaurant.KeyWords,
                IndexImage = findRestaurant.IndexImage,
                ImageAlt = findRestaurant.ImageAlt,
                ImageTitle = findRestaurant.ImageTitle,
                CountryId = findRestaurant.CountryId,
                CityId = findRestaurant.CityId,
                IsSelected = findRestaurant.IsSelected,
                Status = findRestaurant.Status,
            });
        }


        [HttpPost]
        [Route("/Restaurant/Edit")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(RestaurantEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Restaurants");
            }
            var Countries = new List<SelectListItem>(
                        _DbContext.Countries.Select(p => new SelectListItem
                        {
                            Text = p.Name,
                            Value = p.Id.ToString()
                        }).ToList());
            var CitiesInCountry = new List<SelectListItem>(
                _DbContext.Cities.Where(x => x.CountryId == model.CountryId).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Id == model.Id);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var stringImagePath = "";
            if (model.ImageFile != null)
            {
                if (model.ImageFile.Length > 5242848)
                {
                    ModelState.AddModelError("ImageFile", "حجم عکس باید زیر پنج مگابایت باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    ViewBag.ListFirstCities = CitiesInCountry;
                    return View(model);
                }
                if (model.ImageFile.ContentType == "image/png" || model.ImageFile.ContentType == "image/jpg" ||
                    model.ImageFile.ContentType == "image/jpeg" || model.ImageFile.ContentType == "image/gif")
                {
                    stringImagePath = $"Media/Restaurant/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Restaurant");
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "نوع فایل باید به صورت عکس باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    ViewBag.ListFirstCities = CitiesInCountry;
                    return View(model);
                }
            }
            else
                stringImagePath = null;

            var stringVideoPath = "";
            if (model.VideoFile != null)
            {
                if (model.VideoFile.Length > 104856975)
                {
                    ModelState.AddModelError("VideoFile", "حجم ویدیو باید زیر 100 مگابایت باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    ViewBag.ListFirstCities = CitiesInCountry;
                    return View(model);
                }
                if (model.VideoFile.ContentType == "video/mp4" || model.VideoFile.ContentType == "video/wmv")
                {
                    stringVideoPath = $"Media/Restaurant/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Restaurant", "Video");
                }
                else
                {
                    ModelState.AddModelError("VideoFile", "نوع فایل باید به صورت ویدیو باشد");
                    ViewBag.ListCountrySelectList = Countries;
                    ViewBag.ListFirstCities = CitiesInCountry;
                    return View(model);
                }
            }
            else
                stringVideoPath = null;


            if (ModelState.IsValid)
            {
                findRestaurant.Name = model.Name;
                findRestaurant.Slug = repaceSlug;
                findRestaurant.Description = model.Description;
                findRestaurant.KeyWords = model.KeyWords == null ? null : model.KeyWords;
                findRestaurant.Tags = model.Tags == null ? null : model.Tags;
                findRestaurant.VideoAddress = stringVideoPath == null ? findRestaurant.VideoAddress : stringVideoPath;
                findRestaurant.IndexImage = stringImagePath == null ? findRestaurant.IndexImage : stringImagePath;
                findRestaurant.ImageAlt = model.ImageAlt != null ? model.ImageAlt : findRestaurant.Name;
                findRestaurant.ImageTitle = model.ImageTitle != null ? model.ImageTitle : findRestaurant.Name;
                findRestaurant.IsSelected = model.IsSelected;
                findRestaurant.RestaurantSummary = model.RestaurantSummary;
                findRestaurant.AddressAndDetails = model.AddressAndDetails;
                findRestaurant.CountryId = model.CountryId;
                findRestaurant.CityId = model.CityId;
                findRestaurant.Status = RestaurantStatus.WaitingForConfirm;

                _DbContext.Entry(findRestaurant).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Restaurants");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
        [Route("/Restaurant/Delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Id == Id);
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findRestaurant.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findRestaurant.CityId);

            return View(new RestaurantDeleteDto
            {
                Id = findRestaurant.Id,
                Name = findRestaurant.Name,
                IndexImage = findRestaurant.IndexImage,
                ImageAlt = findRestaurant.ImageAlt,
                ImageTitle = findRestaurant.ImageTitle,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
                
            });
        }


        [HttpPost]
        [Route("/Restaurant/Delete")]
        public async Task<IActionResult> Delete(RestaurantDeleteDto model)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(p => p.Id == model.Id);
            if (findRestaurant.Status == RestaurantStatus.Publish)
            { }
            else
            {
                return Redirect("/Profile/Restaurants");
            }
            if (ModelState.IsValid)
            {
                findRestaurant.Status = RestaurantStatus.Delete;
                findRestaurant.DeleteDate = DateTime.Now.ToString();
                findRestaurant.DeleteDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(findRestaurant).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Restaurants");
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
        [Route("/Restaurant/Details/{Id}")]
        public async Task<IActionResult> Details(int Id)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Id == Id);

            var WriterName = await _DbContext.Users.Where(p => p.Id == findRestaurant.WriterId).Select(x => x.FullName).FirstAsync();
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findRestaurant.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findRestaurant.CityId);

            var DetailsHotel = new RestaurantDetailsDto
            {
                Name = findRestaurant.Name,
                Slug = findRestaurant.Slug,
                Description = findRestaurant.Description,
                KeyWords = findRestaurant.KeyWords,
                VideoAddress = findRestaurant.VideoAddress,
                IndexImage = findRestaurant.IndexImage,
                ImageAlt = findRestaurant.ImageAlt,
                ImageTitle = findRestaurant.ImageTitle,
                RegisterDatePersian = findRestaurant.RegisterDatePersian,
                PublishDatePersian = findRestaurant.PublishDatePersian,
                RejectDatePersian = findRestaurant.RejectDatePersian,
                DeleteDatePersian = findRestaurant.DeleteDatePersian,
                WriterName = WriterName,
                IsSelected = findRestaurant.IsSelected,
                Status = findRestaurant.Status,
                Tags = findRestaurant.Tags,
                RestaurantSummary = findRestaurant.RestaurantSummary,
                AddressAndDetails = findRestaurant.AddressAndDetails,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
            };
            ViewBag.StatusName = findRestaurant.Status.Value.ToString();
            return View(DetailsHotel);
        }
        #endregion
    }
}
