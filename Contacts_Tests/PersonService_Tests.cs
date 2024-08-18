using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;

namespace Services.Tests
{
    [TestClass]
    public class PersonServiceTests
    {
        private IPersonService? _personService;
        private PersonDbContext? _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PersonDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use an in-memory database for testing
                .Options;

            _context = new PersonDbContext(options);
            _personService = new PersonService(_context);
        }

        #region AddPerson
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddPerson_WhenRequestIsNull_ThrowsArgumentNullException()
        {
           await _personService?.AddPerson(null)!;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddPerson_WhenNameIsNull_ThrowsArgumentException()
        {
            var request = new PersonAddRequest { Name = null };
            await _personService?.AddPerson(request)!;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddPerson_WhenNameIsDuplicate_ThrowsArgumentException()
        {
            var request1 = new PersonAddRequest { Name = "John Doe" };
            var request2 = new PersonAddRequest { Name = "John Doe" };

            await _personService?.AddPerson(request1)!;

            // This should throw an ArgumentException
            await _personService.AddPerson(request2);
        }

        [TestMethod]
        public async Task AddPerson_WhenRequestIsValid_AddsPersonSuccessfully()
        {
            var request = new PersonAddRequest { Name = "Jane Doe", Email = "test@gmail.com", Phone = "0000000000" };

            var response = await _personService?.AddPerson(request)!;

            Assert.IsNotNull(response);
            Assert.AreEqual("Jane Doe", response.Name);
            Assert.AreNotEqual(Guid.Empty, response.Id);
        }
        #endregion

        #region GetPersonById
        [TestMethod]
        public async Task GetPersonById_WhenIdIsValid_ReturnsPerson()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            var request = new PersonAddRequest { Name = "John Doe", Email = "test@gmail.com", Phone = "0000000000" };
            var addedPerson = await _personService?.AddPerson(request)!;     

            var existingId = addedPerson?.Id;

            var result = await _personService.GetPersonById(existingId);
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [TestMethod]
        public async Task GetPersonById_WhenIdIsNull_ReturnsNull()
        {
            var result = await _personService?.GetPersonById(null)!;
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPersonById_WhenIdDoesNotExist_ReturnsNull()
        {
            var invalidId = Guid.NewGuid();
            var result = await _personService?.GetPersonById(invalidId)!;
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPersonById_WhenListIsEmpty_ReturnsNull()
        {
            var validId = Guid.NewGuid();
            var result = await _personService?.GetPersonById(validId)!;
            Assert.IsNull(result);
        }
        #endregion

        #region GetPersonList
        [TestMethod]
        public async Task GetPersonList_WhenListContainsMultiplePersons_ReturnsAllPersons()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            await _personService?.AddPerson(new PersonAddRequest { Name = "John Doe", Email = "john.doe@gmail.com", Phone = "0000000000" })!;
            await _personService?.AddPerson(new PersonAddRequest { Name = "Jane Doe", Email = "jane.doe@gmail.com", Phone = "0000000000" })!;

            var result = await _personService.GetPersonList();
            Assert.AreEqual(2, result?.Count());
        }

        [TestMethod]
        public async Task GetPersonList_WhenListIsEmpty_ReturnsEmptyList()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            var result = await _personService?.GetPersonList()!;
            Assert.AreEqual(0, result?.Count());
        }

        [TestMethod]
        public async Task GetPersonList_WhenListContainsOnePerson_ReturnsSinglePerson()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            await _personService?.AddPerson(new PersonAddRequest { Name = "John Doe", Email = "john.doe@gmail.com", Phone = "0000000000" })!;
            var result = await _personService.GetPersonList();
            Assert.AreEqual(1, result?.Count());
            Assert.AreEqual("John Doe", result?.First().Name);
        }
        #endregion

        #region GetPersonListFiltered
        [TestMethod]
        public async Task GetPersonListFiltered_ReturnsFullList_WhenSearchByIsNullOrEmpty()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string? searchBy = null;
            string? searchString = null;
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var result = await _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(4, result?.Count());
        }

        [TestMethod]
        public async Task GetPersonListFiltered_FiltersByName()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string searchBy = nameof(Person.Name);
            string searchString = "John";
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = await _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = await _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = await _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(2, result?.Count());
            Assert.IsTrue(result?.All(x => string.IsNullOrEmpty(x.Name) || x.Name.Contains("John", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task GetPersonListFiltered_FiltersByEmail()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string searchBy = nameof(Person.Email);
            string searchString = "example.com";
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = await _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = await _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = await _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(4, result?.Count());
        }

        [TestMethod]
        public async Task GetPersonListFiltered_FiltersByDateOfBirth()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string searchBy = nameof(Person.DateOfBirth);
            string searchString = "01 January 1980";
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = await _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = await _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Phone = "0000000000",Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = await _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(1, result?.Count());
            Assert.AreEqual(new DateTime(1980, 1, 1).ToString("dd MMMM yyyy"), result?.FirstOrDefault()?.DateOfBirth!.Value.ToString("dd MMMM yyyy"));
        }
        [TestMethod]
        public async Task GetPersonListFiltered_ReturnsEmpty_WhenNoMatches()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string searchBy = nameof(Person.Address);
            string searchString = "Nonexistent Address";
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = await _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = await _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = await _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(0, result?.Count());
        }

        [TestMethod]
        public async Task GetPersonListFiltered_ReturnsFullList_WhenInvalidSearchBy()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string searchBy = "InvalidProperty";
            string searchString = "some value";
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = await _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = await _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = await _personService.GetPersonListFiltered(searchBy, searchString);
            var count = result.Count();

            Assert.AreEqual(4, count);
        }
        #endregion

        #region GetPersonListOrdered
        [TestMethod]
        public async Task GetPersonListOrdered_ReturnsOriginalList_WhenOrderByIsNull()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string? orderBy = null;
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();

            var result = _personService.GetPersonListOrdered(_personList, orderBy, null);

            // Assert
            Assert.IsTrue(result.SequenceEqual(_personList));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByName_ASC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.Name);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            // Assert
            var expected = _personList.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByName_DESC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.Name);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByDateOfBirth_ASC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.DateOfBirth);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderBy(x => x.DateOfBirth).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByDateOfBirth_DESC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.DateOfBirth);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.DateOfBirth).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByEmail_ASC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.Email);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public async Task GetPersonListOrdered_OrdersByEmail_DESC()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            string orderBy = nameof(PersonResponse.Email);
            var person_1 = await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) })!;
            var person_2 = await _personService?.AddPerson(new PersonAddRequest() { Name = "George Doe", Phone = "0000000000", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) })!;
            var person_3 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Phone = "0000000000", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) })!;
            var person_4 = await _personService?.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Phone = "0000000000", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) })!;
            var _personList = await _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }
        #endregion

        #region UpdatePerson
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdatePerson_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await _personService?.UpdatePerson(null)!;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdatePerson_ShouldThrowArgumentException_WhenPersonNotFound()
        {
            // Arrange
            var updateRequest = new PersonUpdateRequest
            {
                Id = Guid.NewGuid(), // Non-existing Id
                Name = "John Updated",
                Address = "New Address",
                Gender = Gender.Male,
                Email = "john.updated@example.com",
                CountryId = new Guid(),
                DateOfBirth = new DateTime(1985, 1, 1)
            };

            await _personService?.UpdatePerson(updateRequest)!;
        }

        [TestMethod]
        public async Task UpdatePerson_ShouldUpdatePersonSuccessfully()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            await _personService?.AddPerson(new PersonAddRequest { Name = "John Doe", Phone = "0000000000", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid() })!;
            await _personService?.AddPerson(new PersonAddRequest { Name = "Jane Smith", Phone = "0000000000", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid() })!;
            var _personList = await _personService.GetPersonList();

            var existingPerson = _personList.First();

            var updateRequest = new PersonUpdateRequest
            {
                Id = existingPerson.Id,
                Name = "John Updated",
                Address = "New Address",
                Gender = Gender.Male,
                Email = "john.updated@example.com",
                CountryId = new Guid(),
                DateOfBirth = new DateTime(1985, 1, 1)
            };

            var updatedPersonResponse = await _personService.UpdatePerson(updateRequest);

            Assert.AreEqual(updateRequest.Name, updatedPersonResponse.Name);
            Assert.AreEqual(updateRequest.Address, updatedPersonResponse.Address);
            Assert.AreEqual(updateRequest.Gender.ToString(), updatedPersonResponse.Gender);
            Assert.AreEqual(updateRequest.Email, updatedPersonResponse.Email);
            Assert.AreEqual(updateRequest.CountryId, updatedPersonResponse.CountryId);
            Assert.AreEqual(updateRequest.DateOfBirth, updatedPersonResponse.DateOfBirth);
        }

        #endregion

        #region DeletePerson
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeletePerson_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            await _personService?.DeletePerson(null)!;
        }

        [TestMethod]
        public async Task DeletePerson_ShouldReturnFalse_WhenPersonNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var result = await _personService?.DeletePerson(nonExistentId)!;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeletePerson_ShouldReturnTrue_WhenPersonIsDeleted()
        {
            var allRecords = _context?.Persons.ToList();
            _context?.Persons.RemoveRange(allRecords!);
            _context?.SaveChangesAsync();

            await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid(), Phone = "0000000000" })!;
            await _personService?.AddPerson(new PersonAddRequest() { Name = "Jane Smith", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid(), Phone = "0000000000" })!;
            var _personList = await _personService.GetPersonList();

            var existingPersonId = _personList.First().Id;

            var result = await _personService.DeletePerson(existingPersonId);
            _personList = await _personService.GetPersonList();

            Assert.IsTrue(result);
            Assert.IsFalse(_personList.Any(x => x.Id == existingPersonId));
        }

        [TestMethod]
        public async Task DeletePerson_ShouldDecreaseCount_WhenPersonIsDeleted()
        {
            await _personService?.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid(), Phone = "0000000000" })!;
            await _personService?.AddPerson(new PersonAddRequest() { Name = "Jane Smith", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid(), Phone = "0000000000" })!;
            var _personList = await _personService.GetPersonList();

            var existingPersonId = _personList.First().Id;
            var initialCount = _personList.Count();

            await _personService.DeletePerson(existingPersonId);
            _personList = await _personService.GetPersonList();

            Assert.AreEqual(initialCount - 1, _personList.Count());
        }
        #endregion
    }
}
