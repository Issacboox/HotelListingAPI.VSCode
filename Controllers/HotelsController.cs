using AutoMapper;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Contracts;
using HotelListingAPI.VSCode.Models.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelsRepository _hotelsRepository;
        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository)
        {
            this._mapper = mapper;
            this._hotelsRepository = hotelsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync();
            var records = _mapper.Map<List<GetHotelDto>>(hotels);
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var country = await _hotelsRepository.GetHotelsDetails(id);

            if (country == null)
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<HotelDto>(country);
            return Ok(countryDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDto updateHotelDto)
        {
            if (id != updateHotelDto.HotelId)
            {
                return BadRequest("Invalid Record Id");
            }
            // เมื่อคุณมีเอนทิตีที่ต้องการอัปเดตในฐานข้อมูล, การตั้งค่าคุณสมบัติ State ของเอนทิตีเป็น EntityState.Modified บอก EF Core ว่าเอนทิตีนี้มีการเปลี่ยนแปลงและต้องทำการอัปเดตในฐานข้อมูล

            // _context.Entry(updateCountryDto).State = EntityState.Modified;

            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            //take all the field in updateDto to country
            _mapper.Map(updateHotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // kill run time program
                }
            }
            return NoContent(); // i did all thing but dont show anything
        }

        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto createHotel)
        {
            var hotel = _mapper.Map<Hotel>(createHotel);
            await _hotelsRepository.AddAsync(hotel);
            return CreatedAtAction("GetHotel", new { id = hotel.HotelId }, hotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _hotelsRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exists(id);
        }

    }

}