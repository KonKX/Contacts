using Domain;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required (ErrorMessage = "ID is required.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not in the correct format.")]
        public string? Email { get; set; }

        public PersonResponse ToPersonResponse()
        {
            return new PersonResponse()
            {
                Id = Id,
                Name = Name,
                Gender = Gender.ToString(),
                DateOfBirth = DateOfBirth,
                Address = Address,
                CountryId = CountryId,
                Email = Email
            };
        }
    }
}
