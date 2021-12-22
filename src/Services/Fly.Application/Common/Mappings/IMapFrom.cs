using AutoMapper;

namespace Fly.Application.Common.Mappings
{
    public interface IMapFrom<TEntity>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(TEntity), GetType());
    }
}
