using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Dto.Main.ViewComponent;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Controllers
{
    [Authorize(Roles = "Admin,Special User,Tour Leader User,Normal User")]
    public class TripController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public TripController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ShowTrip
        [HttpGet]
        [Route("Trip/{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdVisitor = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ViewBag.userIdVisitor = userIdVisitor;
            }
            var findTrip = await _DbContext.Trips.FirstOrDefaultAsync(x => x.Slug == slug);
            if (findTrip == null)
            {
                return Redirect("/NotFound");
            }
            var model = new ShowItemDto
            {
                Id = findTrip.Id,
                Slug = slug,
                //CountOfLike = await _DbContext.Likes.Where(p => p.NewsId == findNews.Id && p.StatusLike == StatusLike.Like).CountAsync()
            };
            return View(model);

        }
        #endregion

        #region Create
        [HttpGet]
        [Route("/Profile/Trip/Create")]
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
        [Route("/Profile/Trip/Create")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Create(TripCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Trips");
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
                    stringImagePath = $"Media/Trip/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Trip");
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
                    stringVideoPath = $"Media/Trip/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Trip", "Video");
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
                var NewTrip = new Trip
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
                    PublishDate = DateTime.Now.ToString(),
                    PublishDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
                    WriterId = userId,
                    IsSelected = model.IsSelected,
                    TripStatus = TripStatus.WaitingForConfirm,
                    Tags = model.Tags == null ? null : model.Tags,
                    TripSummary = model.TripSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    TripDuration = model.TripDuration
                };

                await _DbContext.Trips.AddAsync(NewTrip);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Trips");
            }
            return View(model);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Route("/Trip/Edit/{Id}")]
        public IActionResult Edit(int Id)
        {
            var findTrip = _DbContext.Trips.FirstOrDefault(x => x.Id == Id);
            ViewBag.ListCountrySelectList = new List<SelectListItem>(
            _DbContext.Countries.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList());
            ViewBag.ListFirstCities = new List<SelectListItem>(
                _DbContext.Cities.Where(x => x.CountryId == findTrip.CountryId).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            return View(new TripEditDto
            {
                Id = findTrip.Id,
                Name = findTrip.Name,
                Slug = findTrip.Slug,
                Description = findTrip.Description,
                AddressAndDetails = findTrip.AddressAndDetails,
                TripSummary = findTrip.TripSummary,
                Tags = findTrip.Tags,
                KeyWords = findTrip.KeyWords,
                IndexImage = findTrip.IndexImage,
                ImageAlt = findTrip.ImageAlt,
                ImageTitle = findTrip.ImageTitle,
                CountryId = findTrip.CountryId,
                CityId = findTrip.CityId,
                IsSelected = findTrip.IsSelected,
                TripStatus = findTrip.TripStatus,
                TripDuration = findTrip.TripDuration,
            });
        }


        [HttpPost]
        [Route("/Trip/Edit")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(TripEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Trips");
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

            var findTrip = _DbContext.Trips.FirstOrDefault(x => x.Id == model.Id);

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
                    stringImagePath = $"Media/Trip/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Trip");
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
                    stringVideoPath = $"Media/Trip/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Trip", "Video");
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
                findTrip.Name = model.Name;
                findTrip.Slug = repaceSlug;
                findTrip.Description = model.Description;
                findTrip.KeyWords = model.KeyWords == null ? null : model.KeyWords;
                findTrip.Tags = model.Tags == null ? null : model.Tags;
                findTrip.VideoAddress = stringVideoPath == null ? findTrip.VideoAddress : stringVideoPath;
                findTrip.IndexImage = stringImagePath == null ? findTrip.IndexImage : stringImagePath;
                findTrip.ImageAlt = model.ImageAlt != null ? model.ImageAlt : findTrip.Name;
                findTrip.ImageTitle = model.ImageTitle != null ? model.ImageTitle : findTrip.Name;
                findTrip.IsSelected = model.IsSelected;
                findTrip.TripSummary = model.TripSummary;
                findTrip.AddressAndDetails = model.AddressAndDetails;
                findTrip.CountryId = model.CountryId;
                findTrip.CityId = model.CityId;
                findTrip.TripDuration = model.TripDuration;
                findTrip.TripStatus = TripStatus.WaitingForConfirm;

                _DbContext.Entry(findTrip).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Trips");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
        [Route("/Trip/Delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            var findTrip = _DbContext.Trips.FirstOrDefault(x => x.Id == Id);
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findTrip.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findTrip.CityId);

            return View(new TripDeleteDto
            {
                Id = findTrip.Id,
                Name = findTrip.Name,
                IndexImage = findTrip.IndexImage,
                ImageAlt = findTrip.ImageAlt,
                ImageTitle = findTrip.ImageTitle,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
            });
        }


        [HttpPost]
        [Route("/Trip/Delete")]
        public async Task<IActionResult> Delete(TripDeleteDto model)
        {
            var findTrip = _DbContext.Trips.FirstOrDefault(p => p.Id == model.Id);

            if (ModelState.IsValid)
            {
                findTrip.TripStatus = TripStatus.Delete;
                findTrip.DeleteDate = DateTime.Now.ToString();
                findTrip.DeleteDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(findTrip).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Trips");
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
        [Route("/Trip/Details/{Id}")]
        public async Task<IActionResult> Details(int Id)
        {
            var findTrip = _DbContext.Trips.FirstOrDefault(x => x.Id == Id);

            var WriterName = await _DbContext.Users.Where(p => p.Id == findTrip.WriterId).Select(x => x.FullName).FirstAsync();
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findTrip.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findTrip.CityId);
            string TripDurationName = "";

            switch (findTrip.TripDuration)
            {
                case TripDuration.Daily:
                    TripDurationName = "روزانه";
                    break;
                case TripDuration.OneToThreeDays:
                    TripDurationName = "1 روز تا 3 روز";
                    break;
                case TripDuration.Weekly:
                    TripDurationName = "هفتگی";
                    break;
                case TripDuration.MoreThanAWeek:
                    TripDurationName = "بیشتر از یک هفته";
                    break;
                default:
                    // code block
                    break;
            }

            var DetailsHotel = new TripDetailsDto
            {
                Name = findTrip.Name,
                Slug = findTrip.Slug,
                Description = findTrip.Description,
                KeyWords = findTrip.KeyWords,
                VideoAddress = findTrip.VideoAddress,
                IndexImage = findTrip.IndexImage,
                ImageAlt = findTrip.ImageAlt,
                ImageTitle = findTrip.ImageTitle,
                RegisterDatePersian = findTrip.RegisterDatePersian,
                PublishDatePersian = findTrip.PublishDatePersian,
                RejectDatePersian = findTrip.RejectDatePersian,
                DeleteDatePersian = findTrip.DeleteDatePersian,
                WriterName = WriterName,
                IsSelected = findTrip.IsSelected,
                TripStatus = findTrip.TripStatus,
                Tags = findTrip.Tags,
                TripSummary = findTrip.TripSummary,
                AddressAndDetails = findTrip.AddressAndDetails,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
                TripDyrationName = TripDurationName
            };
            ViewBag.StatusName = findTrip.TripStatus.Value.ToString();
            return View(DetailsHotel);
        }
        #endregion
    }
}
