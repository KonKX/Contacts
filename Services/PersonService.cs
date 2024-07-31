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

            return person.ToPersonResponse();
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
                case nameof(Person.Name):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Name) || x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(Person.Gender):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Gender) || x.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(Person.Address):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Address) || x.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(Person.Email):
                    matchingPersonList = fullPersonList.Where(x => (string.IsNullOrEmpty(x.Email) || x.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    break;
                case nameof(Person.DateOfBirth):
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
    }
}
