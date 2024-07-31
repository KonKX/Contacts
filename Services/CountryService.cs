using Domain;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class CountryService : ICountryService
    {
        private readonly List<Country> _countries;

        public CountryService() 
        { 
            _countries = new List<Country>();
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

            if (_countries.Exists(x => x.Name == country.Name))
            {
                throw new ArgumentException(nameof(request.Name));
            }
            else
            {
                _countries.Add(country);
            }

            return country.ToCountryResponse();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            return _countries.Where(x => x.Id == id).FirstOrDefault()?.ToCountryResponse();
        }
        #endregion

        #region GetCountries
        public IEnumerable<CountryResponse> GetCountryList()
        {
            return _countries.Select(x => x.ToCountryResponse()).ToList();
        }
        #endregion
    }
}