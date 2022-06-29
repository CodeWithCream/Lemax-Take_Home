using NetTopologySuite.Geometries;
using Take_Home.Model;

namespace Take_Home.DAL.Interfaces
{
    public interface IHotelRepository
    {
        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        Task<Hotel> GetByIdAsync(long id);

        Task<IEnumerable<Hotel>> GetAllAsync();

        /// <exception cref="typeof(ArgumentException)">Hotel parameters not valid</exception>
        Task InsertAsync(Hotel hotel);

        /// <exception cref="typeof(ArgumentException)">Hotel parameters not valid</exception>
        Task UpdateAsync(long id, string name, float proice, Point geolocation);

        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        Task DeleteAsync(long id);
    }
}