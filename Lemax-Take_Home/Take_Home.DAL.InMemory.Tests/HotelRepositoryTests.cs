using NetTopologySuite.Geometries;
using Newsy_API.DAL.Exceptions;
using Take_Home.DAL.Interfaces;
using Take_Home.Model;

namespace Take_Home.DAL.InMemory.Tests;

[TestClass]
public class HotelRepositoryTests
{
    private static IHotelRepository _hotelRepository = null!;

    private readonly IList<Hotel> _initHotels = new List<Hotel>() {
            new Hotel("hotel1", 55.00f, new Point(15.9485179, 45.7678472)),
            new Hotel("hotel2", 75.50f, new Point(15.9256746, 45.7593127)),
            new Hotel("hotel1", 55.00f, new Point(15.9522229, 45.7516494))};

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _hotelRepository = new HotelRepository();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        await ((HotelRepository)_hotelRepository).InitAsync(_initHotels);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await ((HotelRepository)_hotelRepository).ClearAsnyc();
    }

    [TestMethod]
    public async Task Get_Hotel_By_Id()
    {
        var hotel = await _hotelRepository.GetByIdAsync(2);

        var expectedHotel = _initHotels[1];

        Assert.AreEqual(expectedHotel, hotel);
    }

    [TestMethod, ExpectedException(typeof(NotFoundException))]
    public async Task Getting_Hotel_By_NonExisting_Id_Throws_An_Exception()
    {
        await _hotelRepository.GetByIdAsync(-3);
    }

    [TestMethod]
    public async Task Create_New_Hotel_Successfully()
    {
        var hotelToCreate = new Hotel("hotel 4", 105.75f, new Point(15.9261905, 45.7657717));

        await _hotelRepository.InsertAsync(hotelToCreate);

        var createdHotel = await _hotelRepository.GetByIdAsync(4);

        Assert.AreEqual(hotelToCreate, createdHotel);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Creating_Hotel_With_Empty_Name_ThrowsException()
    {
        var hotelToCreate = new Hotel("hotel 4", 105.75f, new Point(15.9261905, 45.7657717));
        hotelToCreate.Name = "";

        await _hotelRepository.InsertAsync(hotelToCreate);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Creating_Hotel_With_Invalid_Price_ThrowsException()
    {
        var hotelToCreate = new Hotel("hotel 4", 105.75f, new Point(15.9261905, 45.7657717));
        hotelToCreate.Price = 0;

        await _hotelRepository.InsertAsync(hotelToCreate);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Creating_Hotel_With_Invalid_Longitude_ThrowsException()
    {
        var hotelToCreate = new Hotel("hotel 4", 105.75f, new Point(15.9261905, 45.7657717));
        hotelToCreate.Geolocation.X = 1145;

        await _hotelRepository.InsertAsync(hotelToCreate);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Creating_Hotel_With_Invalid_Latitude_ThrowsException()
    {
        var hotelToCreate = new Hotel("hotel 4", 105.75f, new Point(15.9261905, 45.7657717));
        hotelToCreate.Geolocation.Y = 1145;

        await _hotelRepository.InsertAsync(hotelToCreate);
    }

    [TestMethod]
    public async Task Update_Hotel_Successfully()
    {
        var hotelToEdit = await _hotelRepository.GetByIdAsync(3);
        var newName = "best hotel in town";
        var newPrice = 1015f;
        var newGeoLocation = new Point(15.9261905, 45.7657717);

        await _hotelRepository.UpdateAsync(hotelToEdit.Id, newName, newPrice, newGeoLocation);

        var editedHotel = await _hotelRepository.GetByIdAsync(3);

        Assert.AreEqual(newName, editedHotel.Name);
        Assert.AreEqual(newPrice, editedHotel.Price);
        Assert.IsTrue(newGeoLocation.CompareTo(editedHotel.Geolocation) == 0);
    }

    [TestMethod, ExpectedException(typeof(NotFoundException))]
    public async Task Update_Hotel_NonExisting_Hotel_Throws_Exception()
    {
        await _hotelRepository.UpdateAsync(15, "abc", 15f, new Point(15.9261905, 45.7657717));
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Updating_Hotel_With_Empty_Name_ThrowsException()
    {
        var hotelToEdit = await _hotelRepository.GetByIdAsync(3);
        await _hotelRepository.UpdateAsync(hotelToEdit.Id, "", 1015f, hotelToEdit.Geolocation);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Updating_Hotel_With_Invalid_Price_ThrowsException()
    {
        var hotelToEdit = await _hotelRepository.GetByIdAsync(3);
        await _hotelRepository.UpdateAsync(hotelToEdit.Id, hotelToEdit.Name, -3, hotelToEdit.Geolocation);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Updating_Hotel_With_Invalid_Longitude_ThrowsException()
    {
        var hotelToEdit = await _hotelRepository.GetByIdAsync(3);
        await _hotelRepository.UpdateAsync(hotelToEdit.Id, hotelToEdit.Name, hotelToEdit.Price, new Point(1115.9261905, 45.7657717));
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public async Task Updating_Hotel_With_Invalid_Latitude_ThrowsException()
    {
        var hotelToEdit = await _hotelRepository.GetByIdAsync(3);
        await _hotelRepository.UpdateAsync(hotelToEdit.Id, hotelToEdit.Name, hotelToEdit.Price, new Point(15.9261905, -1145.7657717));
    }

    [TestMethod]
    public async Task Delete_Hotel_Successfully()
    {
        var idToDelete = 1;
        await _hotelRepository.DeleteAsync(idToDelete);

        await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await _hotelRepository.GetByIdAsync(idToDelete));
    }

    [TestMethod, ExpectedException(typeof(NotFoundException))]
    public async Task Deleting_Hotel_By_NonExisting_Id_Throws_An_Exception()
    {
        await _hotelRepository.DeleteAsync(10);
    }
}