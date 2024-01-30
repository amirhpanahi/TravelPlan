using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using TravelPlan.Common;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Hotel;
using TravelPlan.Models.Dto.Main.Account;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HotelController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public HotelController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        #region Index
        public async Task<IActionResult> Index(string NameHotel)
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
            if (NameHotel != null)
            {
                var ResultSearch = await SearchByName(NameHotel);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "هتل با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x=>x.Status==HotelStatus.Publish).Select(p => new HotelListDto
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
            var ListHotel = _DbContext.Hotels.OrderByDescending(x => x.Id).Where(x => x.Status == HotelStatus.Publish).Select(p => new HotelListDto
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
        public async Task<IActionResult> Create(HotelCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Hotel/Create");
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
                    Status = HotelStatus.Publish,
                    Tags = model.Tags == null ? null : model.Tags,
                    HotelSummary = model.HotelSummary,
                    AddressAndDetails = model.AddressAndDetails,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                };

                await _DbContext.Hotels.AddAsync(NewNews);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Hotel/index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        [HttpGet]
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
                _DbContext.Cities.Where(x=>x.CountryId == findHotel.CountryId).Select(p => new SelectListItem
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
                HotelSummary=findHotel.HotelSummary,
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
        [RequestFormLimits(MultipartBodyLengthLimit = 115342672)]//110
        public async Task<IActionResult> Edit(HotelEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Hotel/index");
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
                findHotel.VideoAddress = stringVideoPath == null ?findHotel.VideoAddress:stringVideoPath;
                findHotel.IndexImage = stringImagePath == null ? findHotel.IndexImage:stringImagePath;
                findHotel.ImageAlt = model.ImageAlt != null ? model.ImageAlt : findHotel.Name;
                findHotel.ImageTitle = model.ImageTitle != null ? model.ImageTitle : findHotel.Name;
                findHotel.IsSelected = model.IsSelected;
                findHotel.HotelSummary = model.HotelSummary;
                findHotel.AddressAndDetails = model.AddressAndDetails;
                findHotel.CountryId = model.CountryId;
                findHotel.CityId = model.CityId;

                _DbContext.Entry(findHotel).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Hotel/index");
            }
            return View(model);

        }
        #endregion

        #region Delete
        [HttpGet]
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

                return RedirectToAction("Index", "Hotel", new { Areas = "Admin" });
            }

            return View(model);
        }
        #endregion

        #region Details
        [HttpGet]
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

        #region Deleted
        public async Task<IActionResult> Deleted(string NameHotel)
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
            if (NameHotel != null)
            {
                var ResultSearch = await SearchByName(NameHotel);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "هتل با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == HotelStatus.Delete).Select(p => new HotelListDto
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
            var ListHotel = _DbContext.Hotels.OrderByDescending(x => x.Id).Where(x => x.Status == HotelStatus.Delete).Select(p => new HotelListDto
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
        public async Task<IActionResult> Rejected(string NameHotel)
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
            if (NameHotel != null)
            {
                var ResultSearch = await SearchByName(NameHotel);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "هتل با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == HotelStatus.RejectedByAdmin).Select(p => new HotelListDto
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
            var ListHotel = _DbContext.Hotels.OrderByDescending(x => x.Id).Where(x => x.Status == HotelStatus.RejectedByAdmin).Select(p => new HotelListDto
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
        public async Task<IActionResult> WatingForConfirm(string NameHotel)
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
            if (NameHotel != null)
            {
                var ResultSearch = await SearchByName(NameHotel);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "هتل با این مشخصات یافت نشد";
                }

                var listFoundHotel = ResultSearch.Where(x => x.Status == HotelStatus.WaitingForConfirm).Select(p => new HotelListDto
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
            var ListHotel = _DbContext.Hotels.OrderByDescending(x => x.Id).Where(x => x.Status == HotelStatus.WaitingForConfirm).Select(p => new HotelListDto
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
            var HotelFind = _DbContext.Hotels.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.Status = HotelStatus.Publish;
                HotelFind.PublishDate = DateTime.Now.ToString();
                HotelFind.PublishDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Hotel", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion

        #region RejectedByAdmin 
        public async Task<IActionResult> RejectedByAdmin(int Id)
        {
            var HotelFind = _DbContext.Hotels.FirstOrDefault(p => p.Id == Id);
            if (HotelFind != null)
            {
                HotelFind.Status = HotelStatus.RejectedByAdmin;
                HotelFind.RejectDate = DateTime.Now.ToString();
                HotelFind.RejectDatePersian = ConvertorDateTime.ToPersian(DateTime.Now);

                _DbContext.Entry(HotelFind).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("WatingForConfirm", "Hotel", new { Areas = "Admin" });
            }
            return View();
        }
        #endregion



        #region SearchByName
        public async Task<List<HotelListDto>> SearchByName(string FullName)
        {
            var HotelFound = new List<HotelListDto>();

                 HotelFound = _DbContext.Hotels.Where(x => x.Name.Contains(FullName)).Select(p => new HotelListDto
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

            return HotelFound;
        }
        #endregion
    }
}
