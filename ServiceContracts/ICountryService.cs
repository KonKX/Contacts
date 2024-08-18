using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountryService
    {
        /// <summary>
        /// Add a new country to the list.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CountryResponse> AddCountry(CountryAddRequest? request);
        /// <summary>
        /// Get a country by its ID number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<CountryResponse?> GetCountryById(Guid? id);
        /// <summary>
        /// Get the full list of countries.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CountryResponse>> GetCountryList();
    }
}