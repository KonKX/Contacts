using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Contacts.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountryService _countryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PersonsController(IPersonService personService, ICountryService countryService, IWebHostEnvironment environment) 
        { 
            _personService = personService;
            _countryService = countryService;
            _webHostEnvironment = environment;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index()
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                var country_1 = _countryService.AddCountry(new CountryAddRequest() { Name = "Test Country 1" });
                var country_2 = _countryService.AddCountry(new CountryAddRequest() { Name = "Test Country 2" });
                _personService.AddPerson(new PersonAddRequest() { Name = "Test name 1", Address = "test address 1", CountryId = country_1.Id, DateOfBirth = DateTime.Parse("1999-01-01"), Email = "test_email_1@gmail.com", Gender = Gender.Male, Phone = "6954157890" });
                _personService.AddPerson(new PersonAddRequest() { Name = "Test name 2", Address = "test address 2", CountryId = country_2.Id, DateOfBirth = DateTime.Parse("2000-01-02"), Email = "test_email_2@gmail.com", Gender = Gender.Male, Phone = "6974587815" });
                _personService.AddPerson(new PersonAddRequest() { Name = "Test name 3", Address = "test address 3", CountryId = country_1.Id, DateOfBirth = DateTime.Parse("1992-08-03"), Email = "test_email_3@gmail.com", Gender = Gender.Female, Phone = "6934247817" });
                _personService.AddPerson(new PersonAddRequest() { Name = "Test name 4", Address = "test address 4", CountryId = country_2.Id, DateOfBirth = DateTime.Parse("1993-02-04"), Email = "test_email_4@gmail.com", Gender = Gender.Male, Phone = "6935967196" });
            }

            return View(_personService.GetPersonList());
        }

        [Route("persons/about")]
        public IActionResult About()
        {
            return View();
        }
    }
}
