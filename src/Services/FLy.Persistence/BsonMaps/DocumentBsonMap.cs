using MongoDB.Bson.Serialization;

namespace Fly.Persistence.BsonMaps
{
    public static class DocumentBsonMap<T>
    {
        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetDiscriminator(typeof(T).Name);
                });
            }
        }
    }
}