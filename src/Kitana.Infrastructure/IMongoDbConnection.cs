using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Infrastructure
{
    public interface IMongoDbConnection
    {
        public IMongoCollection<T> GetCollection<T>(string name);
    }
}
