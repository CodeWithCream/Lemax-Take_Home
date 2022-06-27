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

        Task InsertAsync(Hotel hotel);

        Task UpdateAsync(long id, string name, float proice, Point geolocation);

        Task DeleteAsync(long id);
    }
}