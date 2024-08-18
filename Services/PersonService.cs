using Domain;
using Microsoft.EntityFrameworkCore;
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
        private readonly PersonDbContext _context;
        public PersonService(PersonDbContext context)
        {
            _context = context;
        }

        #region AddPerson
        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Model validation
            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.Id = Guid.NewGuid();

            if (_context.Persons.Any(x => x.Name == person.Name))
            {
                throw new ArgumentException(nameof(request.Name));
            }
            else
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();
            }
            var personResponse = person.ToPersonResponse();

            return personResponse;
        }
        #endregion

        #region DeletePerson
        public async Task<bool> DeletePerson(Guid? id)
        {
            if (id == null) 
            {
                throw new ArgumentNullException("Please provide an ID");

            }

            var personToRemove = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);
            if (personToRemove != null)
            {
                _context.Persons.Remove(personToRemove);
                await _context.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            return true;
        }
        #endregion

        #region GetPersonById
        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if (id == null)
            {
                return null;
            }

            Person? person = await _context.Persons.Include("Country").FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }
        #endregion

        #region GetPersonList
        public async Task<IEnumerable<PersonResponse>> GetPersonList()
        {
            return await _context.Persons.Include("Country").Select(x => x.ToPersonResponse()).ToListAsync();
        }
        #endregion

        #region GetPersonListFiltered
        public async Task<IEnumerable<PersonResponse>> GetPersonListFiltered(string? searchBy, string? searchString)
        {
            IEnumerable<PersonResponse> fullPersonList = await GetPersonList();
            
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
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            var matchingPerson = await _context.Persons.FirstOrDefaultAsync(x => x.Id == personUpdateRequest.Id);

            if (matchingPerson == null)
            {
                throw new ArgumentException("Person not found.");
            }
            
            matchingPerson.Name = personUpdateRequest.Name;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.Phone = personUpdateRequest.Phone;
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;

            await _context.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }
        #endregion
    }
}
