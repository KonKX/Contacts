using Domain;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class CountryService : ICountryService
    {
        private readonly PersonDbContext _context;
        public CountryService(PersonDbContext db) 
        { 
            _context = db;
        }

        #region AddCountry
        public CountryResponse AddCountry(CountryAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Model validation
            ValidationHelper.ModelValidation(request);

            Country country = request.ToCountry();
            country.Id = Guid.NewGuid();

            if (_context.Countries.Any(x => x.Name == country.Name))
            {
                throw new ArgumentException(nameof(request.Name));
            }
            else
            {
                _context.Countries.Add(country);
                _context.SaveChanges();
            }

            return country.ToCountryResponse();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            return _context.Countries.FirstOrDefault(x => x.Id == id)?.ToCountryResponse();
        }
        #endregion

        #region GetCountries
        public IEnumerable<CountryResponse> GetCountryList()
        {
            return _context.Countries.Select(x => x.ToCountryResponse()).ToList();
        }
        #endregion
    }
}