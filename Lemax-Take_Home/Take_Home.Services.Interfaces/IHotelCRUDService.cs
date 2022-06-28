using Lemax_Take_Home.DTOs;

namespace Take_Home.Services.Interfaces
{
    public interface IHotelCRUDService
    {
        /// <summary>
        /// Searches repository and finds hotel with given id
        /// </summary>
        /// <param name="id">Hotel identifier</param>
        /// <returns>Found hotel parameters mapped to Dto object</returns>
        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        Task<HotelDto> GetByIdAsync(long id);

        /// <summary>
        /// Creates new hotel based on input parameters and saves it to repository
        /// </summary>
        /// <param name="createHotelDto">Parameters of a new hotel</param>
        /// <returns>Created hotel parameters mapped to Dto object</returns>
        /// <exception cref="typeof(ConflictException)">Operation conflict</exception>
        /// <exception cref="typeof(ArgumentException)">Argentuments invalid</exception>
        Task<HotelDto> CreateHotelAsync(CreateEditHotelDto createHotelDto);

        /// <summary>
        /// Updates existing hotel  based on input parameters and saves it to repository
        /// </summary>
        /// <param name="id">Hotel identifier</param>
        /// <param name="editHotelDto">Parameters of a hotel to update or create</param>
        /// <returns>Updated or created hotel parameters mapped to Dto object</returns>
        /// <exception cref="typeof(NotFoundException)">Hotel not found</exception>
        /// <exception cref="typeof(ConflictException)">Operation conflict</exception>
        /// <exception cref="typeof(ArgumentException)">Argentuments invalid</exception>
        Task<HotelDto> UpdateHotelAsync(long id, CreateEditHotelDto editHotelDto);

        /// <summary>
        /// Deletes hotel with given id
        /// </summary>
        /// <param name="id">Hotel identifier</param>
        Task DeleteAsync(long id);
    }
}
