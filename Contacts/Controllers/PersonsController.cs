using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Contacts.Controllers
{
    [Route("[controller]")]
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

        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string? searchBy, string? searchString)
        {
            ViewBag.SearchOptions = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name),"Name"},
                { nameof(PersonResponse.Gender),"Gender"},
                { nameof(PersonResponse.Phone),"Phone"},
                { nameof(PersonResponse.DateOfBirth),"Date of Birth"},
                { nameof(PersonResponse.Age),"Age"},
                { nameof(PersonResponse.Email),"Email"},
                { nameof(PersonResponse.Address),"Address"},
                { nameof(PersonResponse.CountryName),"Country"}
            };
            ViewBag.currentSearchBy = searchBy;
            ViewBag.currentSearchString = searchString;

            IEnumerable<PersonResponse> listOfPersons = _personService.GetPersonListFiltered(searchBy, searchString);
            return View(listOfPersons);
        }
        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Countries = _countryService.GetCountryList();
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = _countryService.GetCountryList();

                //ViewBag.Errors = ModelState.Values
                //    .SelectMany(x => x.Errors)
                //    .Select(x => x.ErrorMessage).ToList();
                return View();
            }

            _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var person = _personService.GetPersonById(id);
            ViewBag.Countries = _countryService.GetCountryList();
            ViewBag.PersonCountryId = person?.CountryId;
            return View(person);
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult Edit(PersonUpdateRequest person)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = _countryService.GetCountryList();

                //ViewBag.Errors = ModelState.Values
                //    .SelectMany(x => x.Errors)
                //    .Select(x => x.ErrorMessage).ToList();
                return View(person.ToPersonResponse());
            }
            _personService.UpdatePerson(person);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public IActionResult Delete(Guid id)
        {
            _personService.DeletePerson(id);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public IActionResult About()
        {
            return View();
        }
    }
}
