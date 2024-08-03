using Domain;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        public PersonService()
        {
            _persons = new List<Person>();
            
        }
        #region AddPerson
        public PersonResponse AddPerson(PersonAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Model validation
            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.Id = Guid.NewGuid();

            if (_persons.Exists(x => x.Name == person.Name))
            {
                throw new ArgumentException(nameof(request.Name));
            }
            else
            {
                _persons.Add(person);
            }
            var personResponse = person.ToPersonResponse();
            return person.ToPersonResponse();
        }
        #endregion

        #region DeletePerson
        public bool DeletePerson(Guid? id)
        {
            if (id == null) 
            {
                throw new ArgumentNullException("Please provide an ID");

            }

            var personToDelete = _persons.FirstOrDefault(x => x.Id == id);
            if (personToDelete == null)
            {
                return false;
            }

            _persons.RemoveAll(x => x.Id == id);

            return true;
        }
        #endregion

        #region GetPersonById
        public PersonResponse? GetPersonById(Guid? id)
        {
            if (id == null)
            {
                return null;
            }

            Person? person = _persons.FirstOrDefault(x => x.Id == id);
            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }
        #endregion

        #region GetPersonList
        public IEnumerable<PersonResponse> GetPersonList()
        {
            return _persons.Select(x => x.ToPersonResponse()).ToList();
        }
        #endregion

        #region GetPersonListFiltered
        public IEnumerable<PersonResponse> GetPersonListFiltered(string? searchBy, string? searchString)
        {
            IEnumerable<PersonResponse> fullPersonList = GetPersonList();
            
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return fullPersonList;
            }

            IEnumerable<PersonResponse> matchingPersonList;

            //For this part reflection could also be used...
            switch (searchBy)
            {
                case nameof(PersonResponse.Name):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Name) || x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Gender) || x.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Address) || x.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(PersonResponse.CountryName):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.CountryName) || x.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Email) || x.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersonList = fullPersonList.Where(x => x.DateOfBirth == null || x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    matchingPersonList = fullPersonList; 
                    break;
            }

            return matchingPersonList;
        }

        public IEnumerable<PersonResponse> GetPersonListOrdered(IEnumerable<PersonResponse> personList, string? orderBy, SortOrderOptions? orderType = SortOrderOptions.ASC)
        {
            if (orderBy == null)
            {
                return personList;
            }

            IEnumerable<PersonResponse> orderedPersonList = (orderBy, orderType)
            switch
            {
                (nameof(Person.Name), SortOrderOptions.ASC) => personList.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Name), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Email), SortOrderOptions.ASC) => personList.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Email), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Address), SortOrderOptions.ASC) => personList.OrderBy(x => x.Address, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Address), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.Address, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.DateOfBirth), SortOrderOptions.ASC) => personList.OrderBy(x => x.DateOfBirth),
                (nameof(Person.DateOfBirth), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.DateOfBirth),
                (nameof(Person.Gender), SortOrderOptions.ASC) => personList.OrderBy(x => x.Gender, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.Gender), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.Gender, StringComparer.OrdinalIgnoreCase),
                (nameof(Person.CountryId), SortOrderOptions.ASC) => personList.OrderBy(x => x.CountryId),
                (nameof(Person.CountryId), SortOrderOptions.DESC) => personList.OrderByDescending(x => x.CountryId),
                _ => personList.OrderBy(x => x.Id)
            };

            return orderedPersonList;

        }
        #endregion

        #region UpdatePerson
        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            var matchingPerson = _persons.FirstOrDefault(x => x.Id == personUpdateRequest.Id);

            if (matchingPerson == null)
            {
                throw new ArgumentException("Person not found.");
            }
            
            matchingPerson.Name = personUpdateRequest.Name;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;

            return matchingPerson.ToPersonResponse();
        }
        #endregion
    }
}
