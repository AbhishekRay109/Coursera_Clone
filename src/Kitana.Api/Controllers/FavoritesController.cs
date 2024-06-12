using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// Controller for managing user favorites.
    /// </summary>
    [Route("favorites")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly FavoritesService _favoritesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoritesController"/> class.
        /// </summary>
        /// <param name="favoritesService">The service responsible for favorites operations.</param>
        public FavoritesController(FavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        /// <summary>
        /// Retrieves the list of user favorites.
        /// </summary>
        /// <returns>An action result containing the list of user favorites.</returns>
        [Permissions("EndUser")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<FavoritesRS>>> GetUserFavorites()
        {
            var favorites = await _favoritesService.GetUserFavoritesAsync();
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched favorites", favorites));
        }

        /// <summary>
        /// Adds a course to the user's favorites.
        /// </summary>
        /// <param name="courseid">The ID of the course to add to favorites.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("EndUser")]
        [HttpPost("/{courseid}")]
        public async Task<IActionResult> AddFavorite(int courseid)
        {
            var result = await _favoritesService.AddFavoriteAsync(courseid);
            if (result != null)
            {
                return Ok(ResponseHandler.HandleSuccess("Successfully inserted a course", result));
            }
            return BadRequest(ResponseHandler.HandleError<string>("NotFound", "Already in favorite"));
        }

        /// <summary>
        /// Removes a course from the user's favorites.
        /// </summary>
        /// <param name="courseid">The ID of the course to remove from favorites.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("EndUser")]
        [HttpDelete("/{courseid}")]
        public async Task<IActionResult> RemoveFavorite(int courseid)
        {
            var result = await _favoritesService.RemoveFavoriteAsync(courseid);
            if (result)
            {
                return Ok(ResponseHandler.HandleSuccess<Favorites>("Successfully removed from favorite"));
            }
            return NotFound(ResponseHandler.HandleError<string>("NotFound", "Favorite not found"));

        }
    }
}
