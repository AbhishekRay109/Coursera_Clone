using Amazon.Runtime.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Infrastructure
{
    public class MongoDbConnection : IMongoDbConnection
    {
        public IMongoDatabase MongoDb { get; set; }
        public MongoDbConnection()
        {
            MongoClient _client = new(Environment.GetEnvironmentVariable("MongoDB_ConnectionString"));
            MongoDb = _client.GetDatabase(Environment.GetEnvironmentVariable("MongoDB_DatabaseName"));
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return MongoDb.GetCollection<T>(name);
        }
    }
}
