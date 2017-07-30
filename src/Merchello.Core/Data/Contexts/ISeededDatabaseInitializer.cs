namespace Merchello.Core.Data.Contexts
{
    using Microsoft.EntityFrameworkCore;

    public interface ISeededDatabaseInitializer<in TContext>
        where TContext : DbContext
    {
        void Initialize(TContext context);
    }
}