namespace Fly.Domain.SeedWork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IUnitOfWork UnitOfWork { get; }
        
    }
}
