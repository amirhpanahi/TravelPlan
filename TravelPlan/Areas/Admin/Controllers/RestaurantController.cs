using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Area.Restaurant;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RestaurantController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public RestaurantController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        #region Index
        public async Task<IActionResult> Index(string NameRestaurant)
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
            if (NameRestaurant != null)
            {
                var ResultSearch = await SearchByName(NameRestaurant);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "رستوران با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == RestaurantStatus.Publish).Select(p => new RestaurantListDto
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
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Restaurants.OrderByDescending(x => x.Id).Where(x => x.Status == RestaurantStatus.Publish).Select(p => new RestaurantListDto
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

            return View(ListHotel);
        }

        #endregion

        #region Create
        [HttpGet]
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
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Create(RestaurantCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Restaurant/Create");
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
                    PublishDate = DateTime.Now.ToString(),
                    PublishDatePersian = ConvertorDateTime.ToPersian(DateTime.Now),
                    WriterId = userId,
                    IsSelected = model.IsSelected,
                    Status = RestaurantStatus.Publish,
                    Tags = model.Tags == null ? null : model.Tags,
                    RestaurantSummary = model.RestaurantSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                };

                await _DbContext.Restaurants.AddAsync(NewRestaurant);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Restaurant/index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(x => x.Id == Id);
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
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(RestaurantEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Restaurant/index");
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

                _DbContext.Entry(findRestaurant).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Restaurant/index");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
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
        public async Task<IActionResult> Delete(RestaurantDeleteDto model)
        {
            var findRestaurant = _DbContext.Restaurants.FirstOrDefault(p => p.Id == model.Id);

            if (ModelState.IsValid)
            {
                findRestaurant.Status = RestaurantStatus.Delete;
                findRestaurant.DeleteDate = DateTime.Now.ToString();
                findRestaurant.DeleteDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(findRestaurant).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Restaurant", new { Areas = "Admin" });
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
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

        #region Deleted
        public async Task<IActionResult> Deleted(string NameRestaurant)
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
            if (NameRestaurant != null)
            {
                var ResultSearch = await SearchByName(NameRestaurant);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "رستوران با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == RestaurantStatus.Delete).Select(p => new RestaurantListDto
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
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Restaurants.OrderByDescending(x => x.Id).Where(x => x.Status == RestaurantStatus.Delete).Select(p => new RestaurantListDto
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

            return View(ListHotel);
        }
        #endregion

        #region Rejected
        public async Task<IActionResult> Rejected(string NameRestaurant)
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
            if (NameRestaurant != null)
            {
                var ResultSearch = await SearchByName(NameRestaurant);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "رستوران با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == RestaurantStatus.RejectedByAdmin).Select(p => new RestaurantListDto
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
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Restaurants.OrderByDescending(x => x.Id).Where(x => x.Status == RestaurantStatus.RejectedByAdmin).Select(p => new RestaurantListDto
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

            return View(ListHotel);
        }
        #endregion

        #region WatingForConfirm
        public async Task<IActionResult> WatingForConfirm(string NameRestaurant)
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
            if (NameRestaurant != null)
            {
                var ResultSearch = await SearchByName(NameRestaurant);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "رستوران با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == RestaurantStatus.WaitingForConfirm).Select(p => new RestaurantListDto
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
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Restaurants.OrderByDescending(x => x.Id).Where(x => x.Status == RestaurantStatus.WaitingForConfirm).Select(p => new RestaurantListDto
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

            return View(ListHotel);
        }
        #endregion

        #region ConfirmByAdmin
        public async Task<IActionResult> ConfirmByAdmin(int Id)
        {
            var HotelFind = _DbContext.Restaurants.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.Status = RestaurantStatus.Publish;
                HotelFind.PublishDate = DateTime.Now.ToString();
                HotelFind.PublishDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Restaurant", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion

        #region RejectedByAdmin 
        public async Task<IActionResult> RejectedByAdmin(int Id)
        {
            var HotelFind = _DbContext.Restaurants.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.Status = RestaurantStatus.RejectedByAdmin;
                HotelFind.RejectDate = DateTime.Now.ToString();
                HotelFind.RejectDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Restaurant", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion



        #region SearchByName
        public async Task<List<RestaurantListDto>> SearchByName(string FullName)
        {
            var RestaurantFound = new List<RestaurantListDto>();

            RestaurantFound = _DbContext.Restaurants.Where(x => x.Name.Contains(FullName)).Select(p => new RestaurantListDto
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

            return RestaurantFound;
        }
        #endregion

    }
}
