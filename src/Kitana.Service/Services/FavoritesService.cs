
using Kitana.Core.Interfaces;
using Kitana.Infrastructure.Repository;
using Kitana.Service.Model;
using Microsoft.AspNetCore.Http;
using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitana.Service.Services
{
    public class FavoritesService
    {
        private readonly IDBFavoritesRepository _favoritesRepository;
        private readonly ISession _session;
        private readonly IHttpContextAccessor httpContextAccessor;
        public FavoritesService(IDBFavoritesRepository favoritesRepository, IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            _favoritesRepository = favoritesRepository;
        }

        public async Task<IEnumerable<FavoritesRS>> GetUserFavoritesAsync()
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var favorites = await _favoritesRepository.GetUserFavoritesAsync(userId);
             var output = new List<FavoritesRS>();
            foreach (var item in favorites)
            {
                var toBeAdded = MapToFavoriteRS(item);
                output.Add(toBeAdded);
            }
            return output;
        }

        public async Task<FavoritesRS> AddFavoriteAsync(int courseId)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var fav = await _favoritesRepository.AddFavoriteAsync(userId, courseId);
            if(fav == null)
            {
                return null;
            }
            return MapToFavoriteRS(fav);
        }

        public async Task<bool> RemoveFavoriteAsync(int courseId)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            return await _favoritesRepository.RemoveFavoriteAsync(userId, courseId);
        }
        // Helper method for mapping between models
        private FavoritesRS MapToFavoriteRS(Favorites favorite)
        {
            if (favorite == null)
            {
                return null;
            }

            return new FavoritesRS
            {
                FavoritesId = favorite.FavoriteId,
                UserName = favorite.User.Username,
                CourseTitle = favorite.Course.Title,
            };
        }
    }
}
