using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Trip;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TripController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public TripController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }


        #region Index
        public async Task<IActionResult> Index(string NameTrip)
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
            if (NameTrip != null)
            {
                var ResultSearch = await SearchByName(NameTrip);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "رستوران با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.TripStatus == TripStatus.Publish).Select(p => new TripListDto
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
                    TripDuration = p.TripDuration,
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Trips.OrderByDescending(x => x.Id).Where(x => x.TripStatus == TripStatus.Publish).Select(p => new TripListDto
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
                TripDuration = p.TripDuration,
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
        public async Task<IActionResult> Create(TripCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Trip/Create");
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
                    TripStatus = TripStatus.Publish,
                    Tags = model.Tags == null ? null : model.Tags,
                    TripSummary = model.TripSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    TripDuration = model.TripDuration
                };

                await _DbContext.Trips.AddAsync(NewTrip);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Trip/index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        [HttpGet]
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
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(TripEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Trip/index");
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

                _DbContext.Entry(findTrip).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Trip/index");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
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

                return RedirectToAction("Index", "Trip", new { Areas = "Admin" });
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
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

        #region Deleted
        public async Task<IActionResult> Deleted(string NameTrip)
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
            if (NameTrip != null)
            {
                var ResultSearch = await SearchByName(NameTrip);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "سفر با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.TripStatus == TripStatus.Delete).Select(p => new TripListDto
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
                    TripDuration = p.TripDuration,
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Trips.OrderByDescending(x => x.Id).Where(x => x.TripStatus == TripStatus.Delete).Select(p => new TripListDto
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
                TripDuration = p.TripDuration,
            }).ToList();

            return View(ListHotel);
        }
        #endregion

        #region Rejected
        public async Task<IActionResult> Rejected(string NameTrip)
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
            if (NameTrip != null)
            {
                var ResultSearch = await SearchByName(NameTrip);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "سفر با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.TripStatus == TripStatus.RejectedByAdmin).Select(p => new TripListDto
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
                    TripDuration = p.TripDuration,
                });

                return View(listFoundHotel);
            }
            var ListHotel = _DbContext.Trips.OrderByDescending(x => x.Id).Where(x => x.TripStatus == TripStatus.RejectedByAdmin).Select(p => new TripListDto
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
                TripDuration = p.TripDuration,
            }).ToList();

            return View(ListHotel);
        }
        #endregion

        #region WatingForConfirm
        public async Task<IActionResult> WatingForConfirm(string NameTrip)
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
            if (NameTrip != null)
            {
                var ResultSearch = await SearchByName(NameTrip);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "سفر با این مشخصات یافت نشد";
                }

                var listFoundTrip = ResultSearch.Where(x => x.TripStatus == TripStatus.WaitingForConfirm).Select(p => new TripListDto
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
                    TripDuration = p.TripDuration,
                });

                return View(listFoundTrip);
            }
            var ListTrip = _DbContext.Trips.OrderByDescending(x => x.Id).Where(x => x.TripStatus == TripStatus.WaitingForConfirm).Select(p => new TripListDto
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
                TripDuration = p.TripDuration,
            }).ToList();

            return View(ListTrip);
        }
        #endregion

        #region ConfirmByAdmin
        public async Task<IActionResult> ConfirmByAdmin(int Id)
        {
            var HotelFind = _DbContext.Trips.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.TripStatus = TripStatus.Publish;
                HotelFind.PublishDate = DateTime.Now.ToString();
                HotelFind.PublishDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Trip", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion

        #region RejectedByAdmin 
        public async Task<IActionResult> RejectedByAdmin(int Id)
        {
            var HotelFind = _DbContext.Trips.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.TripStatus = TripStatus.RejectedByAdmin;
                HotelFind.RejectDate = DateTime.Now.ToString();
                HotelFind.RejectDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Trip", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion



        #region SearchByName
        public async Task<List<TripListDto>> SearchByName(string FullName)
        {
            var TripFound = new List<TripListDto>();

            TripFound = _DbContext.Trips.Where(x => x.Name.Contains(FullName)).Select(p => new TripListDto
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
                TripDuration = p.TripDuration,
            }).ToList();

            return TripFound;
        }
        #endregion
    }
}
