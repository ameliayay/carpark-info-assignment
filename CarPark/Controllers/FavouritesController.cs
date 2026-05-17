using CarPark.DTOs;
using CarPark.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Controllers
{
    [ApiController]
    [Route("api/favourites")]
    [Authorize]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouriteService _service;

        public FavouritesController(IFavouriteService service)
        {
            _service = service;
        }

        // Gets the logged in user's ID from JWT token
        private int CurrentUserId => int.Parse(
            User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

        /// <summary>Get all favourites for the current logged in user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FavouriteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavourites()
        {
            var favs = await _service.GetUserFavouritesAsync(CurrentUserId);
            return Ok(favs);
        }

        /// <summary>Add a carpark to favourites.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddFavourite([FromBody] AddFavouriteRequest request)
        {
            var success = await _service.AddFavouriteAsync(CurrentUserId, request.CarParkNo);
            if (!success)
                return NotFound(new { message = $"Carpark '{request.CarParkNo}' not found." });

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>Remove a carpark from favourites.</summary>
        [HttpDelete("{carParkNo}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavourite(string carParkNo)
        {
            var success = await _service.RemoveFavouriteAsync(CurrentUserId, carParkNo);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}