using AutoMapper;
using GeoSpatialLib;
using Lemax_Take_Home.DTOs;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Take_Home.DAL.Interfaces;
using Take_Home.DTL.Hotel;
using Take_Home.DTL.Pagination;
using Take_Home.Model;
using Take_Home.Services.Interfaces;

[assembly: InternalsVisibleTo("Take_Home.Services.Tests")]
namespace Take_Home.Services
{
    public class BruteForceHotelSearchService : IHotelSearchService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BruteForceHotelSearchService> _logger;

        //max distance (in km) for a hotel to include in search result, should be configured externally, maybe even from a client
        private const double MaxDistanceToFit = 50;

        //caching only one search location per session
        private (GeolocationDto Geolocation, IEnumerable<HotelSearchResultDto> Hotels) _hotelSearchSCache;

        public BruteForceHotelSearchService(IHotelRepository hotelRepository, IMapper mapper, ILogger<BruteForceHotelSearchService> logger)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDto<IEnumerable<HotelSearchResultDto>>> SearchAsync(GeolocationDto searchLocation, PaginationFilterDto paginationFilter)
        {
            ValidateParameters(searchLocation, paginationFilter);

            var orderedList = await SearchHotelsAsync(searchLocation);

            var totalCount = orderedList.Count();
            var totalPages = (int)Math.Ceiling(totalCount * 1.0 / paginationFilter.PageSize);

            IEnumerable<HotelSearchResultDto> pagedResult;
            if (paginationFilter.PageNumber > totalPages)
            {
                var errorMessage = $"Requesting page {paginationFilter.PageNumber} with pagesize of {paginationFilter.PageSize}, but there are not enough articles.";
                _logger.LogWarning(errorMessage);
                throw new ArgumentException(errorMessage, nameof(paginationFilter));
            }
            else
            {
                pagedResult = orderedList
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize);
            }

            return new PagedResultDto<IEnumerable<HotelSearchResultDto>>(pagedResult,
                    paginationFilter.PageNumber,
                    pagedResult.Count(),
                    totalPages,
                    totalCount);
        }

        private async Task<IEnumerable<HotelSearchResultDto>> SearchHotelsAsync(GeolocationDto searchLocation)
        {
            if (searchLocation.Equals(_hotelSearchSCache.Geolocation))
            {
                return _hotelSearchSCache.Hotels;
            }
            else
            {
                var allHotels = await _hotelRepository.GetAllAsync();
                var hotelsWithDistances = await CalculateDistancesAndFitAsync(allHotels, _mapper.Map<Point>(searchLocation));
                var orderedList = hotelsWithDistances
                    .Where(hotel => hotel.Distance <= MaxDistanceToFit)
                    .OrderBy(hotel => hotel.Fit);

                //add to cache
                _hotelSearchSCache = (searchLocation, orderedList);

                return orderedList;
            }
        }

        private async Task<IEnumerable<HotelSearchResultDto>> CalculateDistancesAndFitAsync(IEnumerable<Hotel> hotels, Point searchLocation)
        {
            var result = new List<HotelSearchResultDto>();
            await Parallel.ForEachAsync(hotels, async (hotel, _) =>
            {
                await Task.Run(() =>
                    {
                        var distance = GeoDistance.Calculate(hotel.Geolocation, searchLocation);
                        var fit = CalculateFit(distance, hotel.Price);

                        //map hotel to Dto to save calculated values immediately
                        var hotelSearchResultDto = _mapper.Map<HotelSearchResultDto>(hotel,
                            opt => opt.AfterMap((src, dest) =>
                            {
                                dest.Distance = distance;
                                dest.Fit = fit;
                            }));

                        result.Add(hotelSearchResultDto);
                    }, _);
            });

            return result;
        }

        /// <summary>
        /// Calculate hotel fit based on distance and price. 
        /// Lower number means better fit
        /// </summary>
        /// <param name="distance">Distance in Km</param>
        /// <param name="price">Price</param>
        /// <returns>Calculated fit</returns>
        internal static double CalculateFit(double distance, float price)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("Distance must be greater than 0");
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greatr than 0");
            }

            return Math.Sqrt(distance) * price;
        }

        private void ValidateParameters(GeolocationDto searchLocation, PaginationFilterDto paginationFilter)
        {
            Validate(searchLocation);
            Validate(paginationFilter);
        }

        //Should be extracted to Validator class and used for all validations in the project
        private void Validate<T>(T obj) where T : class
        {
            var ctx = new ValidationContext(obj);

            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, ctx, results, true))
            {
                foreach (var error in results)
                {
                    var invalidMember = error.MemberNames.First();
                    _logger.LogWarning($"Invalid parameter {typeof(T).Name}.{invalidMember} - {error.ErrorMessage}");
                    throw new ArgumentException(error.ErrorMessage, invalidMember);
                }
            }
        }
    }
}
