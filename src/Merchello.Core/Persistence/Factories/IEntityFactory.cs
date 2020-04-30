namespace Merchello.Core.Persistence.Factories
{
	using System;

	[Obsolete("BH: Replace with Automapper", false)]
    internal interface IEntityFactory<TEntity, TDto> 
        where TEntity : class
        where TDto : class
    {
            TEntity BuildEntity(TDto dto);
            TDto BuildDto(TEntity entity);
        }
}
