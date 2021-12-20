using MongoDB.Bson.Serialization.Conventions;
using Fly.Domain.Entities;
using Fly.Persistence.Conventions;

namespace Fly.Persistence.BsonMaps
{
    public static class DatabaseBsonMap
    {
        public static void Map()
        {
            var conventions = new ConventionPack
            {
                new LowerCaseConvention(), 
                new ImmutableTypeClassMapConvention(), 
                new AggregationConvention()
            };

            ConventionRegistry.Register(
                "mongoDbConventions",
                conventions,
                _ => true);

            //Base entity
            EntityBaseBsonMap.Map();

            //Fly context entities
            DocumentBsonMap<Product>.Map();
            DocumentBsonMap<Category>.Map();
        }
    }
}