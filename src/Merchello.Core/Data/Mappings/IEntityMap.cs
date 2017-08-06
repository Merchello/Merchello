namespace Merchello.Core.Data.Mappings
{
    using Microsoft.EntityFrameworkCore;

    public interface IEntityMap
    {
        void Configure(ModelBuilder modelBuilder);
    }
}