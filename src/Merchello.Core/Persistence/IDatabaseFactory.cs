namespace Merchello.Core.Persistence
{
    using System;

    /// <summary>
    /// Used to create the UmbracoDatabase for use in the DatabaseContext
    /// 
    /// </summary>
    public interface IDatabaseFactory : IDisposable
    {
        MerchelloDatabase CreateDatabase();
    }
}