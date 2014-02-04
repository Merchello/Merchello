using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines the StoreSettingRepository
    /// </summary>
    internal interface IStoreSettingRepository : IRepository<Guid, IStoreSetting>
    {
        /// <summary>
        /// Gets the next invoice number (int)
        /// </summary>
        /// <param name="storeSettingKey">Constant Guid Key of the NextInvoiceNumber store setting</param>
        /// <param name="invoicesCount">The number of invoices needing invoice numbers.  Useful when saving multiple new invoices.</param>
        /// <returns></returns>
        int GetNextInvoiceNumber(Guid storeSettingKey, int invoicesCount = 1);
    }
}