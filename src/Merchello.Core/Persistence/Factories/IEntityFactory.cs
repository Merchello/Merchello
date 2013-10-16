﻿namespace Merchello.Core.Persistence.Factories
{
    internal interface IEntityFactory<TEntity, TDto> 
        where TEntity : class
        where TDto : class
    {
            TEntity BuildEntity(TDto dto);
            TDto BuildDto(TEntity entity);
        }
}
