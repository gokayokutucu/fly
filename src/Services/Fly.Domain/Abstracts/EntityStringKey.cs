using Fly.Shopping.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fly.Domain.Abstracts
{
    public abstract class EntityStringKey : IEntity<string>
    {
        public EntityStringKey(long version, bool isDeleted = false, DateTime? lastModifiedDate = null, string modifiedBy = "1", string? id = default)
        {
            Id = string.IsNullOrEmpty(id) ? ObjectId.GenerateNewId().ToString() : id;
            ModifiedBy = modifiedBy;
            LastModifiedDate = lastModifiedDate ?? Clock.Now;
            IsDeleted = isDeleted;
            Version = version;
        }

        public string Id { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastModifiedDate{ get; set; }
        public bool IsDeleted{ get; set; }
        public long Version{ get; set; }
    }
}
