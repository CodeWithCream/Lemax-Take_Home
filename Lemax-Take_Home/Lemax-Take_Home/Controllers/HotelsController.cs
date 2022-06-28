using AutoMapper;
using Lemax_Take_Home.Authorization;
using Lemax_Take_Home.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using Newsy_API.DAL.Exceptions;
using Newtonsoft.Json;
using Take_Home.DAL.Interfaces;
using Take_Home.Model;

namespace Lemax_Take_Home.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IHotelRepository repository, IMapper mapper, ILogger<HotelsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotel(long id)
        {
            try
            {
                var hotel = await _repository.GetByIdAsync(id);
                return new OkObjectResult(_mapper.Map<HotelDto>(hotel));
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Hotel with id={id} does not exist.");
                return NotFound(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while trying to get hotel with id={id}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<HotelDto>> PostHotel(CreateEditHotelDto createHotelDto)
        {
            _logger.LogInformation($"Creating new hotel: {JsonConvert.SerializeObject(createHotelDto)}.");

            var hotelToCreate = _mapper.Map<Hotel>(createHotelDto);

            try
            {
                await _repository.InsertAsync(hotelToCreate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            try
            {
                var createdHotel = await _repository.GetByIdAsync(hotelToCreate.Id);
                var createdHotelDto = _mapper.Map<HotelDto>(createdHotel);
                return CreatedAtAction(nameof(GetHotel), new { id = hotelToCreate.Id }, createdHotelDto);
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogWarning("Someone deleted created hotel.");
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> PutHotel(long id, CreateEditHotelDto editHotelDto)
        {
            _logger.LogInformation($"Updating hotel: {JsonConvert.SerializeObject(editHotelDto)}");

            try
            {
                await _repository.UpdateAsync(id, editHotelDto.Name, editHotelDto.Price, _mapper.Map<Point>(editHotelDto.Geolocation));
                _logger.LogInformation($"Hotel is updated.");
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Hotel with id={id} does not exist.");
                return NotFound(id); //TODO create hotel
            }

            try
            {
                var editedHotel = await _repository.GetByIdAsync(id);
                return new OkObjectResult(_mapper.Map<HotelDto>(editedHotel));
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogWarning("Someone deleted updated hotel.");
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogInformation($"Deleting hotel with id={id}.");

            try
            {
                await _repository.DeleteAsync(id);
                _logger.LogInformation($"Hotel is deleted.");

                return Ok();
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Hotel with id={id} does not exist. Hotel is already deleted.");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
        }
    }
}
