using Domain;
using Microsoft.EntityFrameworkCore;
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
        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
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
                await _context.SaveChangesAsync();
            }

            return country.ToCountryResponse();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            return (await _context.Countries.FirstOrDefaultAsync(x => x.Id == id))?.ToCountryResponse();
        }
        #endregion

        #region GetCountries
        public async Task<IEnumerable<CountryResponse>> GetCountryList()
        {
            return await _context.Countries.Select(x => x.ToCountryResponse()).ToListAsync();
        }
        #endregion
    }
}