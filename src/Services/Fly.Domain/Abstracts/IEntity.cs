namespace Fly.Domain.Abstracts
{
    public interface IEntity<T>
    {
        T Id { get; }

        string ModifiedBy { get; }

        DateTime? LastModifiedDate { get; }
        bool IsDeleted { get; }

        long Version { get; }
    }
}