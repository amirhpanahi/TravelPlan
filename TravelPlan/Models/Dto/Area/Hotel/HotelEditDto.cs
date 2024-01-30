using Resources;
using System.ComponentModel.DataAnnotations;
using TravelPlan.Models.Entities;

namespace TravelPlan.Models.Dto.Area.Hotel
{
    public class HotelEditDto
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public string Name { get; set; }
        public string? AddressAndDetails { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public int CountryId { get; set; }
        public string? Slug { get; set; }
        [Required(ErrorMessageResourceName = nameof(ErrorMessage.Requierd), ErrorMessageResourceType = typeof(ErrorMessage))]
        public int CityId { get; set; }
        public string? IndexImage { get; set; }
        public string? ImageAlt { get; set; }
        public string? ImageTitle { get; set; }
        public string? Description { get; set; }
        public string? VideoAddress { get; set; }
        public string? WriterId { get; set; }
        public bool IsSelected { get; set; }
        public HotelStatus? Status { get; set; }
        public string? DeleteDate { get; set; }
        public string? DeleteDatePersian { get; set; }
        public string? HotelSummary { get; set; }
        public string? KeyWords { get; set; }
        public string? Tags { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
    }
}
