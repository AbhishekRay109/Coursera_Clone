using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBFavoritesRepository
    {
        public Task<IEnumerable<Favorites>> GetUserFavoritesAsync(int userId);
        public Task<Favorites> AddFavoriteAsync(int userId, int courseId);
        public Task<bool> RemoveFavoriteAsync(int userId, int courseId);
    }
}
