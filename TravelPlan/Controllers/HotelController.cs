using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Main.ViewComponent;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Controllers
{
    [Authorize(Roles = "Admin,Special User,Tour Leader User,Normal User")]
    public class HotelController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public HotelController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ShowHotel
        [HttpGet]
        [Route("Hotel/{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdVisitor = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ViewBag.userIdVisitor = userIdVisitor;
            }
            var findHotel = await _DbContext.Hotels.FirstOrDefaultAsync(x => x.Slug == slug);
            if (findHotel == null)
            {
                return Redirect("/NotFound");
            }
            var model = new ShowItemDto
            {
                Id = findHotel.Id,
                Slug = slug,
                //CountOfLike = await _DbContext.Likes.Where(p => p.NewsId == findNews.Id && p.StatusLike == StatusLike.Like).CountAsync()
            };
            return View(model);

        }
        #endregion

        #region Create
        [HttpGet]
        [Route("/Profile/Hotel/Create")]
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
        [Route("/Profile/Hotel/Create")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Create(HotelCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Hotels");
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
                    stringImagePath = $"Media/Hotel/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Hotel");
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
                    stringVideoPath = $"Media/Hotel/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Hotel", "Video");
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
                var NewNews = new Hotel
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
                    Status = HotelStatus.WaitingForConfirm,
                    Tags = model.Tags == null ? null : model.Tags,
                    HotelSummary = model.HotelSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                };

                await _DbContext.Hotels.AddAsync(NewNews);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Hotels");
            }
            return View(model);
        }
        #endregion

        #region Edit

        [HttpGet]
        [Route("/Hotel/Edit/{Id}")]
        public IActionResult Edit(int Id)
        {
            var findHotel = _DbContext.Hotels.FirstOrDefault(x => x.Id == Id);
            ViewBag.ListCountrySelectList = new List<SelectListItem>(
                _DbContext.Countries.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());
            ViewBag.ListFirstCities = new List<SelectListItem>(
                _DbContext.Cities.Where(x => x.CountryId == findHotel.CountryId).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            return View(new HotelEditDto
            {
                Id = findHotel.Id,
                Name = findHotel.Name,
                Slug = findHotel.Slug,
                Description = findHotel.Description,
                AddressAndDetails = findHotel.AddressAndDetails,
                HotelSummary = findHotel.HotelSummary,
                Tags = findHotel.Tags,
                KeyWords = findHotel.KeyWords,
                IndexImage = findHotel.IndexImage,
                ImageAlt = findHotel.ImageAlt,
                ImageTitle = findHotel.ImageTitle,
                CountryId = findHotel.CountryId,
                CityId = findHotel.CityId,
                IsSelected = findHotel.IsSelected,
                Status = findHotel.Status,
            });
        }


        [HttpPost]
        [Route("/Hotel/Edit")]
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(HotelEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Profile/Hotels");
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

            var findHotel = _DbContext.Hotels.FirstOrDefault(x => x.Id == model.Id);

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
                    stringImagePath = $"Media/Hotel/" + await _fileUpload.UploadFileAsync(model.ImageFile, model.Name, "Hotel");
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
                    stringVideoPath = $"Media/Hotel/Video/" + await _fileUpload.UploadFileAsync(model.VideoFile, model.Name, "Hotel", "Video");
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
                findHotel.Name = model.Name;
                findHotel.Slug = repaceSlug;
                findHotel.Description = model.Description;
                findHotel.KeyWords = model.KeyWords == null ? null : model.KeyWords;
                findHotel.Tags = model.Tags == null ? null : model.Tags;
                findHotel.VideoAddress = stringVideoPath == null ? findHotel.VideoAddress : stringVideoPath;
                findHotel.IndexImage = stringImagePath == null ? findHotel.IndexImage : stringImagePath;
                findHotel.ImageAlt = model.ImageAlt != null ? model.ImageAlt : findHotel.Name;
                findHotel.ImageTitle = model.ImageTitle != null ? model.ImageTitle : findHotel.Name;
                findHotel.IsSelected = model.IsSelected;
                findHotel.HotelSummary = model.HotelSummary;
                findHotel.AddressAndDetails = model.AddressAndDetails;
                findHotel.CountryId = model.CountryId;
                findHotel.CityId = model.CityId;
                findHotel.Status = HotelStatus.WaitingForConfirm;

                _DbContext.Entry(findHotel).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Hotels");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
        [Route("/Hotel/Delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            var findHotel = _DbContext.Hotels.FirstOrDefault(x => x.Id == Id);
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findHotel.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findHotel.CityId);

            return View(new HotelDeleteDto
            {
                Id = findHotel.Id,
                Name = findHotel.Name,
                IndexImage = findHotel.IndexImage,
                ImageAlt = findHotel.ImageAlt,
                ImageTitle = findHotel.ImageTitle,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
            });
        }


        [HttpPost]
        [Route("/Hotel/Delete")]
        public async Task<IActionResult> Delete(HotelDeleteDto model)
        {
            var FindHotel = _DbContext.Hotels.FirstOrDefault(p => p.Id == model.Id);

            if (ModelState.IsValid)
            {
                FindHotel.Status = HotelStatus.Delete;
                FindHotel.DeleteDate = DateTime.Now.ToString();
                FindHotel.DeleteDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(FindHotel).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Profile/Hotels");
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
        [Route("/Hotel/Details/{Id}")]
        public async Task<IActionResult> Details(int Id)
        {
            var findHotel = _DbContext.Hotels.FirstOrDefault(x => x.Id == Id);

            var WriterName = await _DbContext.Users.Where(p => p.Id == findHotel.WriterId).Select(x => x.FullName).FirstAsync();
            var CountryFound = _DbContext.Countries.FirstOrDefault(x => x.Id == findHotel.CountryId);
            var CityFound = _DbContext.Cities.FirstOrDefault(x => x.Id == findHotel.CityId);

            var DetailsHotel = new HotelDetailsDto
            {
                Name = findHotel.Name,
                Slug = findHotel.Slug,
                Description = findHotel.Description,
                KeyWords = findHotel.KeyWords,
                VideoAddress = findHotel.VideoAddress,
                IndexImage = findHotel.IndexImage,
                ImageAlt = findHotel.ImageAlt,
                ImageTitle = findHotel.ImageTitle,
                RegisterDatePersian = findHotel.RegisterDatePersian,
                PublishDatePersian = findHotel.PublishDatePersian,
                RejectDatePersian = findHotel.RejectDatePersian,
                DeleteDatePersian = findHotel.DeleteDatePersian,
                WriterName = WriterName,
                IsSelected = findHotel.IsSelected,
                Status = findHotel.Status,
                Tags = findHotel.Tags,
                HotelSummary = findHotel.HotelSummary,
                AddressAndDetails = findHotel.AddressAndDetails,
                CountryName = CountryFound.Name,
                CityName = CityFound.Name,
            };
            ViewBag.StatusName = findHotel.Status.Value.ToString();
            return View(DetailsHotel);
        }
        #endregion
    }
}
