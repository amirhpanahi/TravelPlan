using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TravelPlan.Data;
using TravelPlan.Models.Dto.Other;
using TravelPlan.Models.Entities;

namespace TravelPlan.ViewComponents
{
	public class GeneralSettingsViewComponent : ViewComponent
	{
		private readonly DatabaseContext _dbContext;
		public GeneralSettingsViewComponent(DatabaseContext dataBaseContex)
		{
			_dbContext = dataBaseContex;
		}
		public async Task<IViewComponentResult> InvokeAsync(string SecendPart, string NewsId)
		{
			var settings = await _dbContext.Settings.Select(x => new GeneralSettingDto
			{
				FavIconAddress = "Media/Icon/favicon.ico" /*x.FavIconAddress*/,
				SiteName = x.SiteName + " - " + SecendPart /*(FindNews.Title == null || FindNews.Title == "") ? x.SiteName + " - " + SecendPart : x.SiteName + " - " + FindNews.Title*/,
				SeoDescription = x.SeoDescription /*(FindNews.DescriptionSeo == null || FindNews.DescriptionSeo == "") ? x.SeoDescription : FindNews.DescriptionSeo*/,
			}).FirstAsync();

			return View(settings);
		}
	}
}
