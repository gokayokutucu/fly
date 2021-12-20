using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Fly.Domain.Abstracts;

namespace Fly.Persistence.BsonMaps
{
    public static class EntityBaseBsonMap
    {
        public static void Map()
        {
            //Mapping for class which are derived from Entity object
            //ATTENTION: Don't forget to map non-entity based class
            if (!BsonClassMap.IsClassMapRegistered(typeof(EntityStringKey)))
            {
                BsonClassMap.RegisterClassMap<EntityStringKey>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(c => c.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId))
                        .SetIgnoreIfDefault(true);
                    cm.SetDiscriminator(nameof(EntityStringKey));
                    cm.SetIsRootClass(true);
                });
            }
        }
    }
}
