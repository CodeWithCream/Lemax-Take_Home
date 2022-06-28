using NetTopologySuite.Geometries;
using Take_Home.Model;

namespace Take_Home.DAL.Interfaces
{
    public interface IHotelRepository
    {
        /// <summary>
        /// Finds hotel by its Id
        /// </summary>
        /// <param name="id">Hotel Id</param>
        /// <returns>Found hotel</returns>
        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        Task<Hotel> GetByIdAsync(long id);

        /// <exception cref="typeof(ArgumentException)">Hotel parameters not valid</exception>
        Task InsertAsync(Hotel hotel);

        /// <exception cref="typeof(ArgumentException)">Hotel parameters not valid</exception>
        Task UpdateAsync(long id, string name, float proice, Point geolocation);

        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        Task DeleteAsync(long id);
    }
}