using Domain;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Email { get; set; }
    }

    public static class PersonExtensions
    {
        public static ICountryService? countryService; 
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
                CountryName = countryService?.GetCountryById(person.CountryId)?.Name,
                Email = person.Email
            };
        }
    }
}
