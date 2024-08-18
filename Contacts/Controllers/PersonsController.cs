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
        public async Task<IActionResult> Index(string? searchBy, string? searchString)
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

            var listOfPersons = await _personService.GetPersonListFiltered(searchBy, searchString);
            return View(listOfPersons);
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Countries = await _countryService.GetCountryList();
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await _countryService.GetCountryList();

                //ViewBag.Errors = ModelState.Values
                //    .SelectMany(x => x.Errors)
                //    .Select(x => x.ErrorMessage).ToList();
                return View();
            }
            await _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            var person = await _personService.GetPersonById(id);
            ViewBag.Countries = await _countryService.GetCountryList();
            ViewBag.PersonCountryId = person?.CountryId;
            return View(person);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> Edit(PersonUpdateRequest person)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await _countryService.GetCountryList();

                //ViewBag.Errors = ModelState.Values
                //    .SelectMany(x => x.Errors)
                //    .Select(x => x.ErrorMessage).ToList();
                return View(person.ToPersonResponse());
            }
            await _personService.UpdatePerson(person);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _personService.DeletePerson(id);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public async Task<IActionResult> GetCSV(Guid id)
        {
            var report = await _personService.GetPersonsCSV();
            return File(report, "text/csv", $"contacts_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv");
        }

        [Route("[action]")]
        public IActionResult About()
        {
            return View();
        }
    }
}
