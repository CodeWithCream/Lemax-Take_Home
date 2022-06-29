using AutoMapper;
using Lemax_Take_Home.Controllers;
using Lemax_Take_Home.DTOs;
using Lemax_Take_Home.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newsy_API.DAL.Exceptions;
using Take_Home.DTL.Hotel;
using Take_Home.Services.Interfaces;

namespace Take_Home.Tests.Controllers
{
    [TestClass]
    public class HotelsControllerTests
    {
        private static IMapper _mapper = null!;
        private static Mock<IHotelCRUDService> _hotelCRUDServiceMock = null!;
        private static HotelsController _controller = null!;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _hotelCRUDServiceMock = new Mock<IHotelCRUDService>();
            var hotelSearchServiceMock = new Mock<IHotelSearchService>();
            _controller = new HotelsController(_hotelCRUDServiceMock.Object, hotelSearchServiceMock.Object, _mapper, new NullLogger<HotelsController>());
        }

        [TestMethod]
        public async Task Get_Returns_Product()
        {
            var id = 1;
            var hotelDto = new HotelDto()
            {
                Id = id,
                Name = "hotel1",
                Price = 10.5f,
                Geolocation = new GeolocationDto()
                {
                    Longitude = 15,
                    Latitude = 45
                }
            };

            _hotelCRUDServiceMock.Setup(x => x.GetByIdAsync(id)).Returns(Task.FromResult(hotelDto));

            var actionResult = await _controller.GetHotel(id);
            var contentResult = actionResult.Result as OkObjectResult;
            var foundHotelDto = contentResult.Value as HotelDto;

            Assert.IsNotNull(hotelDto);
            Assert.AreEqual(hotelDto, foundHotelDto);
        }

        [TestMethod]
        public async Task Get_Returns_NotFound_When_Service_Throws_NotFoundException()
        {
            _hotelCRUDServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).Throws<NotFoundException>();

            var actionResult = await _controller.GetHotel(1);

            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundObjectResult));
        }

        ///TODO: test other functions the same way
        ///Not implemented for this excercise because existing tests show the implementation way and purpose
    }
}