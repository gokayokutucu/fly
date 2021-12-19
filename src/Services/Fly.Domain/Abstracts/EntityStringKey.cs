using Fly.Shopping.Helper;
using MongoDB.Bson;

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

        public string Id { get; }
        public string ModifiedBy { get; }
        public DateTime? LastModifiedDate{ get; }
        public bool IsDeleted{ get; }
        public long Version{ get; }
    }
}
