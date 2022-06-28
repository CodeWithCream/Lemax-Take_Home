using AutoMapper;
using Lemax_Take_Home.DTOs;
using Lemax_Take_Home.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using Newsy_API.DAL.Exceptions;
using Take_Home.DAL.Interfaces;
using Take_Home.Model;
using Take_Home.Services.Interfaces;

namespace Take_Home.Services.Tests
{
    [TestClass]
    public class HotelCRUDServiceTests
    {
        private static IMapper _mapper = null!;
        private static Mock<IHotelRepository> _hotelRepositoryMock = null!;
        private static IHotelCRUDService _hotelCRUDService = null!;

        #region Test Data
        const long idToFind = 1;
        readonly Hotel hotelToReturn = new("hotel1", 55.00f, new Point(15.9485179, 45.7678472));

        const long id = 1;
        const string hotelName = "new hotel";
        const float hotelPrice = 100f;
        const double longitude = 15;
        const double latitude = 45;
        readonly CreateEditHotelDto hotelToCreateDto = new()
        {
            Name = hotelName,
            Price = hotelPrice,
            Geolocation = new GeolocationDto()
            {
                Longitude = latitude,
                Latitude = longitude
            }
        };

        readonly Hotel hotelToCreate = new(hotelName, hotelPrice, new Point(longitude, latitude));
        readonly Hotel createdHotelToReturn = new(hotelName, hotelPrice, new Point(longitude, latitude)) { Id = id };

        readonly HotelDto hotelToReturnDto = new()
        {
            Id = id,
            Name = hotelName,
            Price = hotelPrice,
            Geolocation = new GeolocationDto()
            {
                Longitude = longitude,
                Latitude = latitude
            }
        };

        #endregion

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _hotelCRUDService = new HotelCRUDService(_hotelRepositoryMock.Object, _mapper, new NullLogger<HotelCRUDService>());
        }

        [TestMethod]
        public async Task Get_By_Id_Returns_Hotel()
        {
            _hotelRepositoryMock.Setup(x => x.GetByIdAsync(idToFind)).Returns(Task.FromResult(hotelToReturn));

            var hotelDto = await _hotelCRUDService.GetByIdAsync(idToFind);

            Assert.AreEqual(_mapper.Map<HotelDto>(hotelToReturn), hotelDto);
        }

        [TestMethod, ExpectedException(typeof(NotFoundException))]
        public async Task Get_By_Id_Throws_Exception_When_Repository_Throws_Exception()
        {
            _hotelRepositoryMock.Setup(x => x.GetByIdAsync(idToFind)).Throws<NotFoundException>();

            await _hotelCRUDService.GetByIdAsync(idToFind);
        }

        [TestMethod]
        public async Task Create_New_Hotel_Returns_Hotel()
        {
            _hotelRepositoryMock.Setup(x => x.InsertAsync(hotelToCreate)).Verifiable();
            _hotelRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Returns(Task.FromResult(createdHotelToReturn));

            var createdHotelDto = await _hotelCRUDService.CreateHotelAsync(hotelToCreateDto);

            Assert.AreEqual(hotelToReturnDto, createdHotelDto);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public async Task Create_New_Hotel_Throws_Argument_Exception_When_Repository_Throws_Argument_Exception()
        {
            _hotelRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Hotel>())).Throws<ArgumentException>();

            await _hotelCRUDService.CreateHotelAsync(hotelToCreateDto);
        }

        [TestMethod, ExpectedException(typeof(ConflictException))]
        public async Task Create_New_Hotel_Throws_Conflictt_Exception_When_Repository_Throws_NotFound_Exception_After_Creation()
        {
            _hotelRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Hotel>())).Verifiable();
            _hotelRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Throws<NotFoundException>();

            await _hotelCRUDService.CreateHotelAsync(hotelToCreateDto);
        }

        ///TODO: test other functions the same way
        ///Not implemented for this excercise because existing tests show the implementation way and purpose
    }
}