using Domain;
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
        private PersonService _personService;

        public PersonServiceTests()
        {
            _personService = new PersonService();
        }

        #region AddPerson
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPerson_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            _personService.AddPerson(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPerson_WhenNameIsNull_ThrowsArgumentException()
        {
            var request = new PersonAddRequest { Name = null };
            _personService.AddPerson(request);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPerson_WhenNameIsDuplicate_ThrowsArgumentException()
        {
            var request1 = new PersonAddRequest { Name = "John Doe" };
            var request2 = new PersonAddRequest { Name = "John Doe" };

            _personService.AddPerson(request1);

            // This should throw an ArgumentException
            _personService.AddPerson(request2);
        }

        [TestMethod]
        public void AddPerson_WhenRequestIsValid_AddsPersonSuccessfully()
        {
            var request = new PersonAddRequest { Name = "Jane Doe", Email = "test@gmail.com" };

            var response = _personService.AddPerson(request);

            Assert.IsNotNull(response);
            Assert.AreEqual("Jane Doe", response.Name);
            Assert.AreNotEqual(Guid.Empty, response.Id);
        }
        #endregion

        #region GetPersonById
        [TestMethod]
        public void GetPersonById_WhenIdIsValid_ReturnsPerson()
        {
            var request = new PersonAddRequest { Name = "John Doe", Email = "test@gmail.com" };
            var addedPerson = _personService.AddPerson(request);

            var existingId = addedPerson?.Id;

            var result = _personService.GetPersonById(existingId);
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [TestMethod]
        public void GetPersonById_WhenIdIsNull_ReturnsNull()
        {
            var result = _personService.GetPersonById(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetPersonById_WhenIdDoesNotExist_ReturnsNull()
        {
            var invalidId = Guid.NewGuid();
            var result = _personService.GetPersonById(invalidId);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetPersonById_WhenListIsEmpty_ReturnsNull()
        {
            var validId = Guid.NewGuid();
            var result = _personService.GetPersonById(validId);
            Assert.IsNull(result);
        }
        #endregion

        #region GetPersonList
        [TestMethod]
        public void GetPersonList_WhenListContainsMultiplePersons_ReturnsAllPersons()
        {
            _personService.AddPerson(new PersonAddRequest { Name = "John Doe", Email = "john.doe@gmail.com" });
            _personService.AddPerson(new PersonAddRequest { Name = "Jane Doe", Email = "jane.doe@gmail.com" });

            var result = _personService.GetPersonList();
            Assert.AreEqual(2, result?.Count());
        }

        [TestMethod]
        public void GetPersonList_WhenListIsEmpty_ReturnsEmptyList()
        {
            var result = _personService.GetPersonList();
            Assert.AreEqual(0, result?.Count());
        }

        [TestMethod]
        public void GetPersonList_WhenListContainsOnePerson_ReturnsSinglePerson()
        {
            _personService.AddPerson(new PersonAddRequest { Name = "John Doe", Email = "john.doe@gmail.com" });
            var result = _personService.GetPersonList();
            Assert.AreEqual(1, result?.Count());
            Assert.AreEqual("John Doe", result?.First().Name);
        }
        #endregion

        #region GetPersonListFiltered
        [TestMethod]
        public void GetPersonListFiltered_ReturnsFullList_WhenSearchByIsNullOrEmpty()
        {
            string? searchBy = null;
            string? searchString = null;
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(4, result?.Count());
        }

        [TestMethod]
        public void GetPersonListFiltered_FiltersByName()
        {
            string searchBy = nameof(Person.Name);
            string searchString = "John";
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(2, result?.Count());
            Assert.IsTrue(result?.All(x => string.IsNullOrEmpty(x.Name) || x.Name.Contains("John", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void GetPersonListFiltered_FiltersByEmail()
        {
            string searchBy = nameof(Person.Email);
            string searchString = "example.com";
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(4, result?.Count());
        }

        [TestMethod]
        public void GetPersonListFiltered_FiltersByDateOfBirth()
        {
            string searchBy = nameof(Person.DateOfBirth);
            string searchString = "01 January 1980";
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(1, result?.Count());
            Assert.AreEqual(new DateTime(1980, 1, 1).ToString("dd MMMM yyyy"), result?.FirstOrDefault()?.DateOfBirth!.Value.ToString("dd MMMM yyyy"));
        }
        [TestMethod]
        public void GetPersonListFiltered_ReturnsEmpty_WhenNoMatches()
        {
            string searchBy = nameof(Person.Address);
            string searchString = "Nonexistent Address";
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(0, result?.Count());
        }

        [TestMethod]
        public void GetPersonListFiltered_ReturnsFullList_WhenInvalidSearchBy()
        {
            string searchBy = "InvalidProperty";
            string searchString = "some value";
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Johnny Depp", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });

            var result = _personService.GetPersonListFiltered(searchBy, searchString);

            Assert.AreEqual(4, result?.Count());
        }
        #endregion

        #region GetPersonListOrdered
        [TestMethod]
        public void GetPersonListOrdered_ReturnsOriginalList_WhenOrderByIsNull()
        {
            string? orderBy = null;
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();

            var result = _personService.GetPersonListOrdered(_personList, orderBy);

            // Assert
            Assert.IsTrue(result.SequenceEqual(_personList));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByName_ASC()
        {
            string orderBy = nameof(PersonResponse.Name);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            // Assert
            var expected = _personList.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByName_DESC()
        {
            string orderBy = nameof(PersonResponse.Name);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByDateOfBirth_ASC()
        {
            string orderBy = nameof(PersonResponse.DateOfBirth);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderBy(x => x.DateOfBirth).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByDateOfBirth_DESC()
        {
            string orderBy = nameof(PersonResponse.DateOfBirth);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.DateOfBirth).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByEmail_ASC()
        {
            string orderBy = nameof(PersonResponse.Email);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.ASC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetPersonListOrdered_OrdersByEmail_DESC()
        {
            string orderBy = nameof(PersonResponse.Email);
            var person_1 = _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1) });
            var person_2 = _personService.AddPerson(new PersonAddRequest() { Name = "George Doe", Gender = Gender.Male, Address = "345 Elm St", Email = "nope@example.com", DateOfBirth = new DateTime(1985, 2, 5) });
            var person_3 = _personService.AddPerson(new PersonAddRequest() { Name = "Jack Doe", Gender = Gender.Male, Address = "567 Elm St", Email = "test@example.com", DateOfBirth = new DateTime(1970, 5, 13) });
            var person_4 = _personService.AddPerson(new PersonAddRequest() { Name = "Jim Doe", Gender = Gender.Male, Address = "789 Elm St", Email = "whatever@example.com", DateOfBirth = new DateTime(1999, 12, 12) });
            var _personList = _personService.GetPersonList();
            SortOrderOptions orderType = SortOrderOptions.DESC;

            var result = _personService.GetPersonListOrdered(_personList, orderBy, orderType);

            var expected = _personList.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }
        #endregion

        #region UpdatePerson
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdatePerson_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            _personService.UpdatePerson(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdatePerson_ShouldThrowArgumentException_WhenPersonNotFound()
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

            _personService.UpdatePerson(updateRequest);
        }

        [TestMethod]
        public void UpdatePerson_ShouldUpdatePersonSuccessfully()
        {
            _personService.AddPerson(new PersonAddRequest { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid() });
            _personService.AddPerson(new PersonAddRequest { Name = "Jane Smith", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid() });
            var _personList = _personService.GetPersonList();

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

            var updatedPersonResponse = _personService.UpdatePerson(updateRequest);

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
        public void DeletePerson_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            _personService.DeletePerson(null);
        }

        [TestMethod]
        public void DeletePerson_ShouldReturnFalse_WhenPersonNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var result = _personService.DeletePerson(nonExistentId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DeletePerson_ShouldReturnTrue_WhenPersonIsDeleted()
        {
            _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid() });
            _personService.AddPerson(new PersonAddRequest() { Name = "Jane Smith", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid() });
            var _personList = _personService.GetPersonList();

            var existingPersonId = _personList.First().Id;

            var result = _personService.DeletePerson(existingPersonId);
            _personList = _personService.GetPersonList();

            Assert.IsTrue(result);
            Assert.IsFalse(_personList.Any(x => x.Id == existingPersonId));
        }

        [TestMethod]
        public void DeletePerson_ShouldDecreaseCount_WhenPersonIsDeleted()
        {
            _personService.AddPerson(new PersonAddRequest() { Name = "John Doe", Gender = Gender.Male, Address = "123 Elm St", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), CountryId = new Guid() });
            _personService.AddPerson(new PersonAddRequest() { Name = "Jane Smith", Gender = Gender.Female, Address = "456 Oak St", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 2, 2), CountryId = new Guid() });
            var _personList = _personService.GetPersonList();

            var existingPersonId = _personList.First().Id;
            var initialCount = _personList.Count();

            _personService.DeletePerson(existingPersonId);
            _personList = _personService.GetPersonList();

            Assert.AreEqual(initialCount - 1, _personList.Count());
        }
        #endregion
    }
}
