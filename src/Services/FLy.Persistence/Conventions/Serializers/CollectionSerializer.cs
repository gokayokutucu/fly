using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Fly.Common;

namespace Fly.Persistence.Conventions.Serializers
{
    //IN USE: AggregateConvention
    public class CollectionSerializer<T> : SerializerBase<List<T>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, List<T> value)
        {
            context.Writer.WriteStartArray();
            foreach (var bsonDocument in value
                .Select(item => item.ToJson(typeof(T)))
                .Select(jsonDoc => BsonSerializer.Deserialize<BsonDocument>(jsonDoc)))
            {
                BsonSerializer.Serialize(context.Writer, typeof(BsonDocument), bsonDocument);
            }
            context.Writer.WriteEndArray();
        }

        public override List<T> Deserialize(BsonDeserializationContext context,
            BsonDeserializationArgs args)
        {
            object dynamicallyTypedObject = null;
            var type = context.Reader.GetCurrentBsonType();
            if (type != BsonType.Array) return default;

            var genericParameterType = typeof(T);
            if (!IsPrimitive(genericParameterType))
                dynamicallyTypedObject = DynamicType
                    .Make(Guid.NewGuid().ToString())
                    .ExtendWith(typeof(T))
                    .Create();

            context.Reader.ReadStartArray();
            var list = new List<T>();
            while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var item = (T)BsonSerializer.Deserialize(context.Reader, !IsPrimitive(genericParameterType) ? dynamicallyTypedObject!.GetType() : typeof(T));
                list.Add(item);
            }
            context.Reader.ReadEndArray();
            return list;
        }

        private static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type == typeof(Decimal) || type == typeof(String) || type == typeof(DateTime);
        }
    }
}