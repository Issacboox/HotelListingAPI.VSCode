using AutoMapper;
using HotelListing.API.Data;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _context.Countries.ToListAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest("Invalid Record Id");
            }
            // เมื่อคุณมีเอนทิตีที่ต้องการอัปเดตในฐานข้อมูล, การตั้งค่าคุณสมบัติ State ของเอนทิตีเป็น EntityState.Modified บอก EF Core ว่าเอนทิตีนี้มีการเปลี่ยนแปลงและต้องทำการอัปเดตในฐานข้อมูล
            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
        {
            var countryOld = new Country
            {
                Name = createCountry.Name,
                ShortName = createCountry.ShortName
            };

            var country = _mapper.Map<Country>(createCountry);
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}