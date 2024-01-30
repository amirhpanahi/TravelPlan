using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Area.City;
using TravelPlan.Models.Dto.Area.Country;
using TravelPlan.Models.Dto.Area.Users;
using TravelPlan.Models.Entities;
using TravelPlan.Services.FileUploadService;

namespace TravelPlan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlaceController : Controller
    {
        private readonly IFileUploadService _fileUpload;
        private readonly DatabaseContext _DbContext;
        public PlaceController(IFileUploadService fileUpload, DatabaseContext databaseContext)
        {
            _fileUpload = fileUpload;
            _DbContext = databaseContext;
        }

        #region CountryIndex

        //[HttpGet]
        [Route("Admin/Country")]
        [Route("Admin/Country/Index")]
        public async Task<IActionResult> CountryIndex(string NameCountry)
        {
            if (NameCountry != null)
            {
                var ResultSearch = await SearchByName(NameCountry, true);

                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCountryFound = "کشوری با این مشخصات یافت نشد";
                }

                var listFoundCountry = ResultSearch.Select(p => new CountryListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                });

                return View(listFoundCountry);
            }
            var ListCountry = _DbContext.Countries.OrderByDescending(x => x.Id).Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name,
                IndexImage = p.IndexImage,
                ImageAlt = p.ImageAlt,
                ImageTitle = p.ImageTitle,
            }).ToList();

            return View(ListCountry);
        }

        #endregion

        #region CountryCreate

        [HttpGet]
        [Route("Admin/Country/Create")]
        public IActionResult CountryCreate()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/Country/Create")]
        [RequestFormLimits(MultipartBodyLengthLimit = 26214243)]//25
        public async Task<IActionResult> CountryCreate(CountryCreateDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/Country/Create");
            }

            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var stringImagePath = "";
            if (model.IndexImageFile != null)
            {
                if (model.IndexImageFile.Length > 5242848)
                {
                    ModelState.AddModelError("indexImageFile", "حجم عکس باید زیر پنج مگابایت باشد");
                    return View(model);
                }
                if (model.IndexImageFile.ContentType == "image/png" || model.IndexImageFile.ContentType == "image/jpg" ||
                    model.IndexImageFile.ContentType == "image/jpeg" || model.IndexImageFile.ContentType == "image/gif")
                {
                    stringImagePath = $"Media/Country/" + await _fileUpload.UploadFileAsync(model.IndexImageFile, model.Name, "Country");
                }
                else
                {
                    ModelState.AddModelError("indexImageFile", "نوع فایل باید به صورت عکس باشد");
                    return View(model);
                }
            }
            else
                stringImagePath = null;


            if (ModelState.IsValid)
            {
                var NewNews = new Country
                {
                    Name = model.Name,
                    Slug = repaceSlug,
                    ImageAlt = model.ImageAlt != null ? model.ImageAlt : model.Name,
                    ImageTitle = model.ImageTitle != null ? model.ImageTitle : model.Name,
                    IndexImage = stringImagePath,
                    Description = model.Description,
                };

                await _DbContext.Countries.AddAsync(NewNews);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/Country/Index");
            }
            return View(model);
        }
        #endregion

        #region CountryEdit

        [HttpGet]
        [Route("~/Admin/Country/Edit/{Id}")]
        public IActionResult CountryEdit(int Id)
        {
            var FindCountry = _DbContext.Countries.FirstOrDefault(p => p.Id == Id);
            return View(new CountryEditDto
            {
                Id = FindCountry.Id,
                Name = FindCountry.Name,
                Description = FindCountry.Description,
                Slug = FindCountry.Slug,
                IndexImage = FindCountry.IndexImage,
                ImageAlt = FindCountry.ImageAlt,
                ImageTitle = FindCountry.ImageTitle,
            });
        }

        [HttpPost]
        [Route("~/Admin/Country/Edit")]
        [RequestFormLimits(MultipartBodyLengthLimit = 26214243)]//25
        public async Task<IActionResult> CountryEdit(CountryEditDto model)
        {
            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var FindCountry = _DbContext.Countries.FirstOrDefault(p => p.Id == model.Id);
            var stringPath = "";
            if (model.File != null)
            {
                if (model.File.Length > 5242848)
                {
                    ModelState.AddModelError("IndexImage", "حجم عکس باید زیر پنج مگابایت باشد");

                    return View(new CountryEditDto
                    {
                        Name = FindCountry.Name,
                        Description = FindCountry.Description,
                        Slug = FindCountry.Slug,
                        IndexImage = FindCountry.IndexImage,
                        ImageAlt = FindCountry.ImageAlt,
                        ImageTitle = FindCountry.ImageTitle,
                    });

                }
                if (model.File.ContentType == "image/png" || model.File.ContentType == "image/jpg" ||
                    model.File.ContentType == "image/jpeg" || model.File.ContentType == "image/gif")
                {
                    stringPath = await _fileUpload.UploadFileAsync(model.File, model.Name, "Country");
                }
                else
                {
                    ModelState.AddModelError("IndexImage", "نوع فایل باید به صورت عکس باشد");
                    return View(new CountryEditDto
                    {
                        Name = FindCountry.Name,
                        Description = FindCountry.Description,
                        Slug = FindCountry.Slug,
                        IndexImage = FindCountry.IndexImage,
                        ImageAlt = FindCountry.ImageAlt,
                        ImageTitle = FindCountry.ImageTitle,
                    });
                }
            }
            if (ModelState.IsValid)
            {
                FindCountry.Name = model.Name;
                FindCountry.Slug = repaceSlug;
                FindCountry.Description = model.Description;
                FindCountry.ImageAlt = model.ImageAlt != null ? model.ImageAlt : model.Name;
                FindCountry.ImageTitle = model.ImageTitle != null ? model.ImageTitle : model.Name;
                FindCountry.IndexImage = model.File == null ? FindCountry.IndexImage : $"Media/Country/{stringPath}";

                _DbContext.Entry(FindCountry).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Country", new { Areas = "Admin" });
            }

            return View(model);
        }

        #endregion

        #region CountryDelete

        [HttpGet]
        [Route("~/Admin/Country/Delete/{Id}")]
        public IActionResult CountryDelete(int Id)
        {
            var FindCountry = _DbContext.Countries.FirstOrDefault(p => p.Id == Id);
            return View(new CountryDeleteDto
            {
                Id = FindCountry.Id,
                
                Name = FindCountry.Name,
                Description = FindCountry.Description,
                IndexImage = FindCountry.IndexImage,
                ImageAlt = FindCountry.ImageAlt,
                ImageTitle = FindCountry.ImageTitle,
            });
        }

        [HttpPost]
        [Route("~/Admin/Country/Delete")]
        public async Task<IActionResult> CountryDelete(CountryDeleteDto model)
        {
            var FindCountry = _DbContext.Countries.FirstOrDefault(p => p.Id == model.Id);

            if (ModelState.IsValid)
            {
                _DbContext.Countries.Remove(FindCountry);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Country", new { Areas = "Admin" });
            }

            return View(model);
        }

        #endregion

        #region CountryDetails

        [HttpGet]
        [Route("~/Admin/Country/Details/{Id}")]
        public async Task<IActionResult> CountryDetails(int Id)
        {
            var FindCountry = _DbContext.Countries.FirstOrDefault(p => p.Id == Id);

            return View(new CountryEditDto
            {
                Id = FindCountry.Id,
                Name = FindCountry.Name,
                Description = FindCountry.Description,
                Slug = FindCountry.Slug,
                IndexImage = FindCountry.IndexImage,
                ImageAlt = FindCountry.ImageAlt,
                ImageTitle = FindCountry.ImageTitle,
            });
        }

        #endregion



        #region CityIndex

        //[HttpGet]
        [Route("Admin/City")]
        [Route("Admin/City/Index")]
        public async Task<IActionResult> CityIndex(string NameCity)
            {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            if (NameCity != null)
            {
                var ResultSearch = await SearchByName(NameCity, false);
                if (ResultSearch.Count == 0)
                {
                    ViewBag.NoCityFound = "شهری با این مشخصات یافت نشد";
                }

                var listFoundCity = ResultSearch.Select(p => new CityListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsSelected = p.IsSelected,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                });

                return View(listFoundCity);
            }
            var ListCity = _DbContext.Cities.OrderByDescending(x => x.Id).Select(p => new CityListDto
            {
                Id = p.Id,
                Name = p.Name,
                IndexImage = p.IndexImage,
                ImageAlt = p.ImageAlt,
                ImageTitle = p.ImageTitle,
                CountryId = p.CountryId,
                IsSelected = p.IsSelected,
            }).ToList();

            return View(ListCity);
        }

        #endregion

        #region CityCreate

        [HttpGet]
        [Route("~/Admin/City/Create")]
        public IActionResult CityCreate()
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
        [Route("~/Admin/City/Create")]
        [RequestFormLimits(MultipartBodyLengthLimit = 26214243)]//25
        public async Task<IActionResult> CityCreate(CityCreateDto model) 
        {
            if (model == null)
            {
                return Redirect("/Admin/City/Create");
            }

            ViewBag.ListCountrySelectList = new List<SelectListItem>(
                // به صورت استاتیک گرفته شده است ParentId در اینجا 
                _DbContext.Countries.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var stringImagePath = "";
            if (model.IndexImageFile != null)
            {
                if (model.IndexImageFile.Length > 5242848)
                {
                    ModelState.AddModelError("IndexImageFile", "حجم عکس باید زیر پنج مگابایت باشد");
                    return View(model);
                }
                if (model.IndexImageFile.ContentType == "image/png" || model.IndexImageFile.ContentType == "image/jpg" ||
                    model.IndexImageFile.ContentType == "image/jpeg" || model.IndexImageFile.ContentType == "image/gif")
                {
                    stringImagePath = $"Media/City/" + await _fileUpload.UploadFileAsync(model.IndexImageFile, model.Name, "City");
                }
                else
                {
                    ModelState.AddModelError("IndexImageFile", "نوع فایل باید به صورت عکس باشد");
                    return View(model);
                }
            }
            else
                stringImagePath = null;


            if (ModelState.IsValid)
            {
                var NewCity = new City
                {
                    Name = model.Name,
                    Slug = repaceSlug,
                    ImageAlt = model.ImageAlt != null ? model.ImageAlt : model.Name,
                    ImageTitle = model.ImageTitle != null ? model.ImageTitle : model.Name,
                    IndexImage = stringImagePath,
                    Description = model.Description,
                    IsSelected = model.IsSelected,
                    CountryId = model.CountryId,
                };

                await _DbContext.Cities.AddAsync(NewCity);
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/City/Index");
            }
            return View(model);
        }

        #endregion

        #region CityEdit

        [HttpGet]
        [Route("~/Admin/City/Edit/{Id}")]
        public IActionResult CityEdit(int Id)
        {
            var findCity = _DbContext.Cities.FirstOrDefault(x => x.Id == Id);
            ViewBag.ListCountrySelectList = new List<SelectListItem>(
                _DbContext.Countries.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());
            return View(new CityEditDto
            {
                Id = findCity.Id,
                Name = findCity.Name,
                Description = findCity.Description,
                Slug = findCity.Slug,
                ImageAlt = findCity.ImageAlt,
                ImageTitle= findCity.ImageTitle,
                IndexImageAddress = findCity.IndexImage,
                CountryId = findCity.CountryId,
                IsSelected = findCity.IsSelected,
            });
        }

        [HttpPost]
        [Route("~/Admin/City/Edit")]
        public async Task<IActionResult> CityEdit(CityEditDto model)
        {
            if (model == null)
            {
                return Redirect("/Admin/City/index");
            }

            ViewBag.ListCountrySelectList = new List<SelectListItem>(
                // به صورت استاتیک گرفته شده است ParentId در اینجا 
                _DbContext.Countries.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList());

            string repaceSlug = "";
            if (model.Slug != null)
                repaceSlug = model.Slug.Replace(" ", "-").Replace("/", "-");
            else
                repaceSlug = model.Name.Replace(" ", "-").Replace("/", "-");

            var FindCity = _DbContext.Cities.FirstOrDefault(p => p.Id == model.Id);
            var stringPath = "";
            if (model.IndexImageFile != null)
            {
                if (model.IndexImageFile.Length > 5242848)
                {
                    ModelState.AddModelError("IndexImageFile", "حجم عکس باید زیر پنج مگابایت باشد");

                    return View(new CityEditDto
                    {
                        Name = FindCity.Name,
                        Description = FindCity.Description,
                        Slug = FindCity.Slug,
                        IndexImageAddress = FindCity.IndexImage,
                        ImageAlt = FindCity.ImageAlt,
                        ImageTitle = FindCity.ImageTitle,
                        IsSelected = FindCity.IsSelected,
                        CountryId = FindCity.CountryId,
                    });

                }
                if (model.IndexImageFile.ContentType == "image/png" || model.IndexImageFile.ContentType == "image/jpg" ||
                    model.IndexImageFile.ContentType == "image/jpeg" || model.IndexImageFile.ContentType == "image/gif")
                {
                    stringPath = await _fileUpload.UploadFileAsync(model.IndexImageFile, model.Name, "City");
                }
                else
                {
                    ModelState.AddModelError("IndexImageFile", "نوع فایل باید به صورت عکس باشد");
                    return View(new CityEditDto
                    {
                        Name = FindCity.Name,
                        Description = FindCity.Description,
                        Slug = FindCity.Slug,
                        IndexImageAddress = FindCity.IndexImage,
                        ImageAlt = FindCity.ImageAlt,
                        ImageTitle = FindCity.ImageTitle,
                        IsSelected = FindCity.IsSelected,
                        CountryId = FindCity.CountryId,
                    });
                }
            }
            if (ModelState.IsValid)
            {
                FindCity.CountryId = model.CountryId;
                FindCity.Name = model.Name;
                FindCity.Slug = repaceSlug;
                FindCity.Description = model.Description;
                FindCity.ImageAlt = model.ImageAlt != null ? model.ImageAlt : model.Name;
                FindCity.ImageTitle = model.ImageTitle != null ? model.ImageTitle : model.Name;
                FindCity.IndexImage = model.IndexImageFile == null ? FindCity.IndexImage : $"Media/City/{stringPath}";
                FindCity.IsSelected = model.IsSelected;
                

                _DbContext.Entry(FindCity).State = EntityState.Modified;
                await _DbContext.SaveChangesAsync();

                return Redirect("/Admin/City/Index");
            }

            return View(model);
        }

        #endregion

        #region CityDelete

        [HttpGet]
        [Route("~/Admin/City/Delete/{Id}")]
        public IActionResult CityDelete(int Id)
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var FindCity = _DbContext.Cities.FirstOrDefault(p => p.Id == Id);
            return View(new CityDeleteDto
            {
                Id = FindCity.Id,
                Name = FindCity.Name,
                Description = FindCity.Description,
                IndexImage = FindCity.IndexImage,
                ImageAlt = FindCity.ImageAlt,
                ImageTitle = FindCity.ImageTitle,
                CountryId = FindCity.CountryId,
            });
        }

        [HttpPost]
        [Route("~/Admin/City/Delete")]
        public async Task<IActionResult> CityDelete(CityDeleteDto model)
        {
            var FindCity = _DbContext.Cities.FirstOrDefault(p => p.Id == model.Id);

            if (ModelState.IsValid)
            {
                _DbContext.Cities.Remove(FindCity);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Index", "City", new { Areas = "Admin" });
            }

            return View(model);
        }

        #endregion

        #region CityDetails

        [HttpGet]
        [Route("~/Admin/City/Details/{Id}")]
        public async Task<IActionResult> CityDetails(int Id)
        {
            ViewBag.ListCountry = _DbContext.Countries.Select(p => new CountryListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            var FindCity = _DbContext.Cities.FirstOrDefault(p => p.Id == Id);

            return View(new CityEditDto
            {
                Id = FindCity.Id,
                Name = FindCity.Name,
                Description = FindCity.Description,
                Slug = FindCity.Slug,
                IndexImageAddress = FindCity.IndexImage,
                ImageAlt = FindCity.ImageAlt,
                ImageTitle = FindCity.ImageTitle,
                CountryId = FindCity.CountryId,
                IsSelected = FindCity.IsSelected,
            });
        }

        #endregion



        #region SearchByName
        public async Task<List<SearchByNameCC>> SearchByName(string FullName, bool IsCountry)
        {
            if (IsCountry == true)
            {
                var CountryFound = _DbContext.Countries.Where(x => x.Name.Contains(FullName)).Select(p => new SearchByNameCC
                {
                    Id = p.Id,
                    Name = p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                }).ToList();

                return CountryFound;
            }
            else
            {
                var CityFound = _DbContext.Cities.Where(x => x.Name.Contains(FullName)).Select(p => new SearchByNameCC
                {
                    Id = p.Id,
                    Name = p.Name,
                    IndexImage = p.IndexImage,
                    ImageAlt = p.ImageAlt,
                    ImageTitle = p.ImageTitle,
                    IsSelected = p.IsSelected,
                }).ToList();

                return CityFound;
            }
        }
        #endregion

        #region Service GetCity
        [HttpGet]
        [Route("Service/GetCityByCountry")]
        public JsonResult GetCityByCountry(int CountryId)
        {
            return Json(_DbContext.Cities.Where(x => x.CountryId == CountryId).ToList());
        }
        #endregion
    }
}
