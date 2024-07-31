using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// Add a new person to the contact list.
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
        /// Get the full contact list of persons.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonList();
        /// <summary>
        /// Get the filtered contact list of persons.
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonListFiltered(string? searchBy, string? searchString);
        /// <summary>
        /// Get the ordered list of persons in contact list.
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public IEnumerable<PersonResponse> GetPersonListOrdered(IEnumerable<PersonResponse> personList, string? orderBy, SortOrderOptions? orderType);
       /// <summary>
       /// Update a person in contact list.
       /// </summary>
       /// <param name="personUpdateRequest"></param>
       /// <returns></returns>
        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        /// <summary>
        /// Delete a person from contact list.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePerson(Guid? id);
    }
}
