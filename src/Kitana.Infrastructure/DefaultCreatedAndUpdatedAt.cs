using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Infrastructure
{
    public static class DefaultCreatedAndUpdatedAt
    {

        public static T SetCreatedAtAndUpdatedAt<T>(this DbContext context, T entity)
        {
            var now = DateTime.Now;
            if (entity == null)
            {
                return entity;
            }

            var entry = context.Entry(entity);

            if (entry.State == EntityState.Added)
            {
                SetProperty(entity, "CreatedAt", now);
                SetProperty(entity, "UpdatedAt", now);
            }

            if (entry.State == EntityState.Modified)
            {
                SetProperty(entity, "UpdatedAt", now);
            }
            return entity;
        }

        private static void SetProperty<T>(T entity, string propertyName, object value)
        {
            var propertyInfo = entity.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(entity, value, null);
            }
        }
    }

}
