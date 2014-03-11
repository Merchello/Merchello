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

        /// <summary>
        /// Gets the next order number (int)
        /// </summary>
        /// <param name="storeSettingKey">Constant Guid Key of the NextOrderNumber store setting</param>
        /// <param name="ordersCount">The number of orders needing invoice orders.  Useful when saving multiple new orders.</param>
        /// <returns></returns>
        int GetNextOrderNumber(Guid storeSettingKey, int ordersCount = 1);
    }
}