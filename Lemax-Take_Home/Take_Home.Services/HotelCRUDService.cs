using AutoMapper;
using Lemax_Take_Home.DTOs;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newsy_API.DAL.Exceptions;
using Newtonsoft.Json;
using Take_Home.DAL.Interfaces;
using Take_Home.Model;
using Take_Home.Services.Interfaces;

namespace Take_Home.Services
{
    public class HotelCRUDService : IHotelCRUDService
    {
        private readonly IHotelRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelCRUDService> _logger;

        public HotelCRUDService(IHotelRepository repository, IMapper mapper, ILogger<HotelCRUDService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HotelDto> GetByIdAsync(long id)
        {
            _logger.LogInformation($"Find hotel with id {id}");

            var hotel = await _repository.GetByIdAsync(id);

            _logger.LogInformation($"Found hotel: name={hotel.Name}, price={hotel.Price}, geolocation=({hotel.Geolocation.X}, {hotel.Geolocation.Y}).");

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task<HotelDto> CreateHotelAsync(CreateEditHotelDto createHotelDto)
        {
            var hotelToCreate = _mapper.Map<Hotel>(createHotelDto);

            _logger.LogInformation($"Creating new hotel: name={hotelToCreate.Name}, price={hotelToCreate.Price}, geolocation=({hotelToCreate.Geolocation.X}, {hotelToCreate.Geolocation.Y}).");

            await _repository.InsertAsync(hotelToCreate);

            try
            {
                var createdHotel = await _repository.GetByIdAsync(hotelToCreate.Id);
                var createdHotelDto = _mapper.Map<HotelDto>(createdHotel);

                _logger.LogInformation($"Created hotel: {JsonConvert.SerializeObject(createdHotelDto)}.");

                return createdHotelDto;
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogWarning("Someone deleted created hotel.");
                throw new ConflictException();
            }
        }

        public async Task<HotelDto> UpdateHotelAsync(long id, CreateEditHotelDto hotelToEditDto)
        {
            _logger.LogInformation($"Updating hotel with if {id} and parameters: name={hotelToEditDto.Name}, price={hotelToEditDto.Price}, " +
                $"geolocation=({hotelToEditDto.Geolocation.Longitude}, {hotelToEditDto.Geolocation.Latitude}).");

            await _repository.UpdateAsync(id, hotelToEditDto.Name, hotelToEditDto.Price, _mapper.Map<Point>(hotelToEditDto.Geolocation));

            _logger.LogInformation($"Hotel is updated.");

            try
            {
                var editedHotel = await _repository.GetByIdAsync(id);
                return _mapper.Map<HotelDto>(editedHotel);
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogWarning("Someone deleted updated hotel.");
                throw new ConflictException();
            }
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation($"Deleting hotel with id={id}.");

            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogInformation("Someone already deleted the hotel.");
            }

            _logger.LogInformation($"Hotel is deleted.");
        }
    }
}