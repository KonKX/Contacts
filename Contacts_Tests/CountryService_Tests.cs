using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Runtime.CompilerServices;

namespace Services.Tests
{
    [TestClass]
    public class CountryServiceTests
    {
        private ICountryService? _countryService;
        private PersonDbContext? _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PersonDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use an in-memory database for testing
                .Options;

            _context = new PersonDbContext(options);
            _countryService = new CountryService(_context);
        }

        #region AddCountry
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddCountry_RequestIsNull_ThrowsArgumentNullException()
        {
            await _countryService?.AddCountry(null)!;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddCountry_RequestNameIsNull_ThrowsArgumentException()
        {
            var request = new CountryAddRequest { Name = null };
            await _countryService?.AddCountry(request)!;
        }

        [TestMethod]
        public async Task AddCountry_ValidRequest_AddsCountrySuccessfully()
        {
            var request = new CountryAddRequest { Name = "TestCountry" };
            var response = await _countryService?.AddCountry(request)!;

            Assert.IsNotNull(response);
            Assert.AreEqual("TestCountry", response.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            var request1 = new CountryAddRequest { Name = "DuplicateCountry" };
            var request2 = new CountryAddRequest { Name = "DuplicateCountry" };

            await _countryService?.AddCountry(request1)!;

            // This should throw an ArgumentException
            await _countryService?.AddCountry(request2)!;
        }
        #endregion

        #region GetCountryList
        [TestMethod]
        public async Task GetCountryList_WhenListIsEmpty_ReturnsEmptyList()
        {
            var allRecords = _context?.Countries.ToList();
            _context?.Countries.RemoveRange(allRecords!);
            await _context?.SaveChangesAsync()!;

            var count = _context?.Countries.Count();

            var result = await _countryService?.GetCountryList()!;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task GetCountryList_WhenListHasCountries_ReturnsCountryList()
        {
            var allRecords = _context?.Countries.ToList();
            _context?.Countries.RemoveRange(allRecords!);
            await _context?.SaveChangesAsync()!;

            var request1 = new CountryAddRequest { Name = "Country1" };
            var request2 = new CountryAddRequest { Name = "Country2" };

            _countryService?.AddCountry(request2);
            _countryService?.AddCountry(request1);

            var result = await _countryService?.GetCountryList()!;
            var count = result.Count();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, count);
            Assert.AreEqual("Country1", result.ToList()[0].Name);
            Assert.AreEqual("Country2", result.ToList()[1].Name);
        }
        #endregion
        
        #region GetCountryById
        [TestMethod]
        public async Task GetCountryById_WhenIdIsNull_ReturnsNull()
        {
            var result =await _countryService?.GetCountryById(null)!;
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetCountryById_WhenIdDoesNotExist_ReturnsNull()
        {
            var nonExistentId = Guid.NewGuid();
            var result = await _countryService?.GetCountryById(nonExistentId)!;
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetCountryById_WhenIdExists_ReturnsCountryResponse()
        {
            var request = new CountryAddRequest { Name = "Country1" };
            var addedCountry = await _countryService?.AddCountry(request)!;

            var existingId = addedCountry?.Id;

            var result = await _countryService?.GetCountryById(existingId)!;

            Assert.IsNotNull(result);
            Assert.AreEqual(addedCountry?.Name, result.Name);
        }
        #endregion
    }
}
