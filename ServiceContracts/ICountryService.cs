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
        public CountryResponse AddCountry(CountryAddRequest? request);
        /// <summary>
        /// Get a country by its ID number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CountryResponse? GetCountryById(Guid? id);
        /// <summary>
        /// Get the full list of countries.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CountryResponse> GetCountryList();
    }
}