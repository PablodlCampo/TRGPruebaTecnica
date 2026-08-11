using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// Provides access to the MongoDB client and database used by the application.
    /// </summary>
    public sealed class MongoService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoService"/> class.
        /// </summary>
        /// <param name="options">
        /// Configuration options containing the MongoDB connection string
        /// and database name.
        /// </param>
        public MongoService(IOptions<MongoDbSettings> options)
        {
            RegisterBsonClasses();
            MongoClient = new MongoClient(options.Value.ConnectionString);
            Database = MongoClient.GetDatabase(options.Value.MongoDbDatabaseName);
        }

        /// <summary>
        /// Gets the MongoDB client.
        /// </summary>
        public MongoClient MongoClient { get; }

        /// <summary>
        /// Gets the MongoDB database used by the application.
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Configures the BSON serializers required by the application.
        /// </summary>
        private static void RegisterBsonClasses()
        {
            BsonSerializer.RegisterSerializer(
                new GuidSerializer(GuidRepresentation.Standard));
        }
    }
}
