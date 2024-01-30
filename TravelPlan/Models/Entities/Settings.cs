namespace TravelPlan.Models.Entities
{
	public class Settings
	{
		public int Id { get; set; }
		public string? SiteName { get; set; }
		public string? Title { get; set; }
		public string? LogoAddress { get; set; }
		public string? FavIconAddress { get; set; }

		public string? SeoDescription { get; set; }
		public string? KeyWords { get; set; }

		public string? ScriptHeader { get; set; }

		public string? TextForFooter { get; set; }
		public string? FooterMenu { get; set; }
		public string? SocialMedia { get; set; }
		public string? Permissions { get; set; }
		public string? CopyrightText { get; set; }
		public string? ScriptFooter { get; set; }
	}
}
