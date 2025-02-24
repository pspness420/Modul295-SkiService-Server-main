using MongoDB.Driver;
using MongoDB.Bson;
using System;
using SkiServiceManagement.Models;

namespace SkiServiceManagement.Data 
{
    public class MangoDBContext
    {
        private readonly IMongoDatabase _database;
        public MangoDBContext()
        {
            var connectionString = "mongodb+srv://admin:2233@cluster0.5ndzy.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("MongoDB URI is missing in environment variables.");
            }
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("SkiServiceDB");
        }

        public IMongoCollection<Benutzer> Benutzer
        {
            get
            {
                return _database.GetCollection<Benutzer>("Benutzer");
            }
        }

        public IMongoCollection<BsonDocument> Serviceauftraege
        {
            get
            {
                return _database.GetCollection<BsonDocument>("Serviceauftraege");
            }
        }
    }
}