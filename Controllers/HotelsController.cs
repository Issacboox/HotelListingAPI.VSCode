using AutoMapper;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Contracts;
using HotelListingAPI.VSCode.Models;
using HotelListingAPI.VSCode.Models.ApiResponse; // Ensure this namespace is correct
using HotelListingAPI.VSCode.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelsRepository _hotelsRepository;

        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository)
        {
            this._mapper = mapper;
            this._hotelsRepository = hotelsRepository;
        }

        [HttpGet("test")]
        public async void GetData()
        {
            await Task.Delay(10000);
            Console.Write("Delay 5000");
            await Task.Delay(1000);
            Console.Write("Delay 1000");
        }

        // Changed the return type from void to Task
    

        [HttpGet("all")]
        [EnableQuery]
        public async Task<ActionResult<IQueryable<GetHotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync();
            var records = _mapper.Map<List<GetHotelDto>>(hotels);
            return Ok(records.AsQueryable());
        }


        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<GetHotelDto>>> GetPagedHotels([FromQuery] QueryParameters queryParameters)
        {
            var pagedHotelResults = await _hotelsRepository.GetAllAsync<GetHotelDto>(queryParameters);
            return Ok(pagedHotelResults);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<HotelDto>>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetHotelsDetails(id);

            if (hotel == null)
            {
                return NotFound(new ApiResponse<HotelDto>(StatusCodes.Status404NotFound, false, "Hotel not found", null));
            }

            var hotelDto = _mapper.Map<HotelDto>(hotel);
            return Ok(new ApiResponse<HotelDto>(StatusCodes.Status200OK, true, "Hotel retrieved successfully", hotelDto));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult<ApiResponse<object>>> PutHotel(int id, UpdateHotelDto updateHotelDto)
        {
            if (id != updateHotelDto.HotelId)
            {
                return BadRequest(new ApiResponse<object>(StatusCodes.Status400BadRequest, false, "Invalid Record Id", null));
            }

            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null)
            {
                return NotFound(new ApiResponse<object>(StatusCodes.Status404NotFound, false, "Hotel not found", null));
            }

            _mapper.Map(updateHotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    return NotFound(new ApiResponse<object>(StatusCodes.Status404NotFound, false, "Hotel not found", null));
                }
                else
                {
                    throw; // rethrow the exception
                }
            }

            return Ok(new ApiResponse<object>(StatusCodes.Status200OK, true, "Hotel updated successfully", null)); // Return Ok with success message
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult<ApiResponse<HotelDto>>> PostHotel(CreateHotelDto createHotel)
        {
            var hotel = _mapper.Map<Hotel>(createHotel);
            await _hotelsRepository.AddAsync(hotel);
            var hotelDto = _mapper.Map<HotelDto>(hotel);
            return CreatedAtAction("GetHotel", new { id = hotel.HotelId }, new ApiResponse<HotelDto>(StatusCodes.Status201Created, true, "Hotel created successfully", hotelDto));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null)
            {
                return NotFound(new ApiResponse<object>(StatusCodes.Status404NotFound, false, "Hotel not found", null));
            }

            await _hotelsRepository.DeleteAsync(id);
            return Ok(new ApiResponse<object>(StatusCodes.Status200OK, true, "Hotel deleted successfully", null)); // Return Ok with success message
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}
