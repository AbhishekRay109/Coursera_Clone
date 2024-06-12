using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitana.Infrastructure.Repository
{
    public class FavoritesRepository : IDBFavoritesRepository
    {
        private readonly SkillForgeDBContext _context;

        public FavoritesRepository(SkillForgeDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorites>> GetUserFavoritesAsync(int userId)
        {
            return await _context.Favorites.Include(x => x.Course).Include(y => y.User)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Favorites> AddFavoriteAsync(int userId, int courseId)
        {
            var existingFavorite = await _context.Favorites.Include(x => x.Course).Include(y => y.User)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CourseId == courseId);

            if (existingFavorite != null)
            {
                return null;
            }

            var favorite = new Favorites { UserId = userId, CourseId = courseId , UpdatedAt = DateTime.Now};
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            var fav = await _context.Favorites
              .Include(x => x.Course).Include(b => b.User).FirstOrDefaultAsync(f => f.UserId == userId && f.CourseId == courseId);

            return fav;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, int courseId)
        {
            var favorite = await _context.Favorites.Include(x => x.Course).Include(b => b.User)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CourseId == courseId);

            if (favorite == null)
            {
                return false; // Favorite not found
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
