using AutoMapper;
using Lemax_Take_Home.DTOs;
using Lemax_Take_Home.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using Take_Home.DAL.Interfaces;
using Take_Home.DTL;
using Take_Home.Model;
using Take_Home.Services.Interfaces;
using Take_Home.Services.Tests.Helpers;

namespace Take_Home.Services.Tests
{
    [TestClass]
    public class BruteForceHotelSearchServiceTests
    {
        private static IMapper _mapper = null!;
        private static IHotelSearchService _hotelSearchService = null!;

        #region Test Data

        private readonly GeolocationDto _defaultSearchLocation = new()
        {
            Latitude = 45.7693306,
            Longitude = 15.9500395
        };

        private readonly PaginationFilterDto _defaultPaginationFilter = new()
        {
            PageNumber = 1,
            PageSize = 10
        };

        private static IEnumerable<Hotel> _allHotels = null!;

        #endregion

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var hotelRepositoryMock = new Mock<IHotelRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _hotelSearchService = new BruteForceHotelSearchService(hotelRepositoryMock.Object, _mapper, new NullLogger<BruteForceHotelSearchService>());

            _allHotels = JsonExtensions.LoadFromFileWithGeoJson<List<Hotel>>("../../../test_hotel_data.json");

            hotelRepositoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_allHotels));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Search_Hotels_With_Invalid_Search_Longitude_Throws_Exception()
        {
            var searchLocation = new GeolocationDto()
            {
                Latitude = 1000,
                Longitude = 15
            };

            await _hotelSearchService.SearchAsync(searchLocation, _defaultPaginationFilter);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Search_Hotels_With_Invalid_Search_Latitude_Throws_Exception()
        {
            var searchLocation = new GeolocationDto()
            {
                Latitude = 45,
                Longitude = 1500
            };

            await _hotelSearchService.SearchAsync(searchLocation, _defaultPaginationFilter);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Search_Hotels_With_Invalid_Page_Number_Throws_Exception()
        {
            var paginationfilter = new PaginationFilterDto()
            {
                PageNumber = -1,
                PageSize = 10
            };

            await _hotelSearchService.SearchAsync(_defaultSearchLocation, paginationfilter);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Search_Hotels_With_Invalid_Page_Size_Throws_Exception()
        {
            var paginationfilter = new PaginationFilterDto()
            {
                PageNumber = 1,
                PageSize = 0
            };

            await _hotelSearchService.SearchAsync(_defaultSearchLocation, paginationfilter);
        }

        [TestMethod]
        public void Calculate_Fit_Returns_Number()
        {
            var distance1 = 0.5;
            var price1 = 100;

            Assert.AreEqual(CalculateFit(distance1, price1), BruteForceHotelSearchService.CalculateFit(distance1, price1));

            var distance2 = 0.7;
            var price2 = 80;

            Assert.AreEqual(CalculateFit(distance2, price2), BruteForceHotelSearchService.CalculateFit(distance2, price2));

            var distance3 = 1500;
            var price3 = 30;

            Assert.AreEqual(CalculateFit(distance3, price3), BruteForceHotelSearchService.CalculateFit(distance3, price3));

            var distance4 = 7;
            var price4 = 30;

            Assert.AreEqual(CalculateFit(distance4, price4), BruteForceHotelSearchService.CalculateFit(distance4, price4));
        }

        private static double CalculateFit(double distance, float price)
        {
            return Math.Sqrt(distance) * price;
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Calculate_Fit_Throws_Exception_When_Called_With_Invalid_Distance()
        {
            BruteForceHotelSearchService.CalculateFit(0, 10);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Calculate_Fit_Throws_Exception_When_Called_With_Invalid_Pricee()
        {
            BruteForceHotelSearchService.CalculateFit(10, 0);
        }

        [TestMethod]
        public async Task Search_Hotels_Returns_Result()
        {
            var hotelsSearchResult = await _hotelSearchService.SearchAsync(_defaultSearchLocation, _defaultPaginationFilter);

            Assert.AreEqual(_defaultPaginationFilter.PageSize, hotelsSearchResult.PageSize);
            Assert.AreEqual(_defaultPaginationFilter.PageNumber, hotelsSearchResult.PageNumber);
            Assert.IsTrue(hotelsSearchResult.Data.Any());
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Search_Hotels_With_Page_Size_Too_Big_Throws_Exception()
        {
            var paginationfilter = new PaginationFilterDto()
            {
                PageNumber = 50,
                PageSize = 10
            };

            await _hotelSearchService.SearchAsync(_defaultSearchLocation, paginationfilter);
        }


        //TODO: add more testing, ie.
        //right hotels are returned
        //if cache is empty, filled afer search
        //if data in cache, return
        //if new location sent, calculate again
        //removed all with distance greater than 50
        //paging calculated ok

    }
}
