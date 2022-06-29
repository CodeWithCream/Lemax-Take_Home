using Lemax_Take_Home.DTOs;
using Take_Home.DTL.Hotel;
using Take_Home.DTL.Pagination;

namespace Take_Home.Services.Interfaces
{
    public interface IHotelSearchService
    {
        /// <summary>
        /// Searches hotels and orderes them based on distance and price. 
        /// Hotels that are cheaper and closer to search location are closer to the top of the list. 
        /// Hotels that are more expensive and further away are closer to the bottom of the list. 
        /// </summary>
        /// <returns>List of hotels orderd by distance and price</returns>
        Task<PagedResultDto<IEnumerable<HotelSearchResultDto>>> SearchAsync(GeolocationDto searchLocation, PaginationFilterDto paginationFilter);
    }
}
