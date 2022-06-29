using AutoMapper;
using Lemax_Take_Home.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy_API.DAL.Exceptions;
using Newtonsoft.Json;
using Take_Home.DTL.Hotel;
using Take_Home.DTL.Pagination;
using Take_Home.Services.Interfaces;

namespace Lemax_Take_Home.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelCRUDService _hotelCRUDService;
        private readonly IHotelSearchService _hotelSearchService;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IHotelCRUDService hotelCRUDService, IHotelSearchService hotelSearchService, IMapper mapper, ILogger<HotelsController> logger)
        {
            _hotelCRUDService = hotelCRUDService;
            _hotelSearchService = hotelSearchService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> GetHotel(long id)
        {
            try
            {
                _logger.LogInformation($"Find hotel with id {id}");

                var hotelDto = await _hotelCRUDService.GetByIdAsync(id);

                _logger.LogInformation($"Found hotel: {JsonConvert.SerializeObject(hotelDto)}.");

                return new OkObjectResult(hotelDto);
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

            try
            {
                var createdHotelDto = await _hotelCRUDService.CreateHotelAsync(createHotelDto);

                _logger.LogInformation($"Created hotel: {JsonConvert.SerializeObject(createHotelDto)}.");

                return CreatedAtAction(nameof(GetHotel), new { id = createdHotelDto.Id }, createdHotelDto);
            }
            catch (ConflictException e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
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
                var updatedHotelDto = await _hotelCRUDService.UpdateHotelAsync(id, editHotelDto);

                _logger.LogInformation($"Updated hotel: {JsonConvert.SerializeObject(updatedHotelDto)}.");

                return new OkObjectResult(updatedHotelDto);
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Hotel with id={id} does not exist.");
                return NotFound(id);
            }
            catch (ConflictException e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
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
                await _hotelCRUDService.DeleteAsync(id);

                _logger.LogInformation($"Hotel is deleted.");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDto<IEnumerable<HotelDto>>>> GetHotels(
            [FromQuery] GeolocationDto currentLocation,
            [FromQuery] PaginationFilterDto paginationFilter)
        {
            _logger.LogInformation($"Search hotels near {currentLocation.Longitude},{currentLocation.Latitude}. (Page {paginationFilter.PageNumber})");

            try
            {
                var hotels = await _hotelSearchService.SearchAsync(currentLocation, paginationFilter);

                return new OkObjectResult(hotels);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
