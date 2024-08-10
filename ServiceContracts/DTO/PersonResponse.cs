using Domain;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Display(Name = "Gender")]
        public string? Gender { get; set; }
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Age")]
        public double? Age { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        [Display(Name = "Country")]
        public string? CountryName { get; set; }
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }

    public static class PersonExtensions
    {
        public static ICountryService? _countryService;
        
        public static void Initialize(ICountryService? countryService)
        {
            _countryService = countryService;
        }

        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                Id = person.Id,
                Name = person.Name,
                Gender = person.Gender,
                Phone = person.Phone,
                DateOfBirth = person.DateOfBirth,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Address = person.Address,
                CountryId = person.CountryId,
                CountryName = _countryService?.GetCountryById(person.CountryId)?.Name,
                Email = person.Email
            };
        }
    }
}
