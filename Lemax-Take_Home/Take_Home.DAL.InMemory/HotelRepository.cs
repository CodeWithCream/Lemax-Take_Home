using NetTopologySuite.Geometries;
using Newsy_API.DAL.Exceptions;
using System.Runtime.CompilerServices;
using Take_Home.DAL.Interfaces;
using Take_Home.Model;


[assembly: InternalsVisibleTo("Take_Home.DAL.InMemory.Tests")]
namespace Take_Home.DAL.InMemory
{
    public class HotelRepository : IHotelRepository
    {
        private static long _lastId = 0;
        private readonly IDictionary<long, Hotel> _hotels = new Dictionary<long, Hotel>();

        public async Task<Hotel> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                if (_hotels.TryGetValue(id, out Hotel? hotel))
                {
                    return hotel;
                }
                else
                {
                    throw new NotFoundException();
                }
            });
        }

        public async Task InsertAsync(Hotel hotel)
        {
            await Task.Run(() =>
            {
                Hotel.Validate(hotel.Name, hotel.Price, hotel.Geolocation);
                SetId(hotel);
                _hotels.Add(hotel.Id, hotel);
            });
        }

        public async Task UpdateAsync(long id, string name, float price, Point geolocation)
        {
            Hotel.Validate(name, price, geolocation);
            var hotelToEdit = await GetByIdAsync(id);
            hotelToEdit.Name = name;
            hotelToEdit.Price = price;
            hotelToEdit.Geolocation = geolocation;
        }

        public async Task DeleteAsync(long id)
        {
            await Task.Run(() =>
            {
                if (_hotels.TryGetValue(id, out Hotel? hotel))
                {
                    _hotels.Remove(id);
                }
                else
                {
                    throw new NotFoundException();
                }
            });
        }

        internal async Task InitAsync(IList<Hotel> hotels)
        {
            await Task.Run(() =>
            {
                foreach (Hotel hotel in hotels)
                {
                    SetId(hotel);
                    _hotels.Add(hotel.Id, hotel);
                };
            });
        }

        internal async Task ClearAsnyc()
        {
            await Task.Run(() =>
            {
                _hotels.Clear();
                _lastId = 0;
            });
        }

        private void SetId(Hotel hotel)
        {
            hotel.Id = ++_lastId;
        }
    }
}