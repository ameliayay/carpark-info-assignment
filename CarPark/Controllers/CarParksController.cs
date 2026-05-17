using CarPark.DTOs;
using CarPark.Interfaces;
using CarPark.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers
{
    [ApiController]
    [Route("api/carparks")]
    [AllowAnonymous]
    public class CarParksController : ControllerBase
    {
        private readonly ICarParkService _service;

        public CarParksController(ICarParkService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get filtered list of carparks.
        /// All filters are optional and combinable.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CarParkDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCarParks(
            [FromQuery] bool? freeParking,
            [FromQuery] bool? nightParking,
            [FromQuery] decimal? minVehicleHeight)
        {
            var filter = new CarParkFilter
            {
                FreeParking = freeParking,
                NightParking = nightParking,
                MinVehicleHeight = minVehicleHeight
            };

            var results = await _service.GetCarParksAsync(filter);
            return Ok(results);
        }

        /// <summary>Get a single carpark by its carpark number.</summary>
        [HttpGet("{carParkNo}")]
        [ProducesResponseType(typeof(CarParkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCarParkNo(string carParkNo)
        {
            var result = await _service.GetByCarParkNoAsync(carParkNo);
            if (result is null) return NotFound();
            return Ok(result);
        }
    }
}
