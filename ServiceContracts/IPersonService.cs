using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// Add a new person to the list.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PersonResponse AddPerson(PersonAddRequest? request);
        /// <summary>
        /// Get a person by their ID number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PersonResponse? GetPersonById(Guid? id);
        /// <summary>
        /// Get the full list of persons.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonList();
        /// <summary>
        /// Get a filtered list of persons.
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonListFiltered(string? searchBy, string? searchString);
        /// <summary>
        /// Get an ordered list of persons.
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonListOrdered(IEnumerable<PersonResponse> personList, string? orderBy, SortOrderOptions? orderType);
    }
}
