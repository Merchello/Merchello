namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines the StoreSettingRepository
    /// </summary>
    internal interface IStoreSettingRepository : IRepository<Guid, IStoreSetting>
    {
        /// <summary>
        /// Gets the next invoice number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextInvoiceNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="invoicesCount">The number of invoices needing invoice numbers.  Useful when saving multiple new invoices.</param>
        /// <returns>The next invoice number</returns>
        int GetNextInvoiceNumber(Guid storeSettingKey, Func<int> validate, int invoicesCount = 1);

        /// <summary>
        /// Gets the next order number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextOrderNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="ordersCount">The number of orders needing invoice orders.  Useful when saving multiple new orders.</param>
        /// <returns>The next order number</returns>
        int GetNextOrderNumber(Guid storeSettingKey, Func<int> validate, int ordersCount = 1);


        /// <summary>
        /// Gets the next shipment number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextOrderNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="shipmentsCount">The number of orders needing invoice orders.  Useful when saving multiple new orders.</param>
        /// <returns>The next order number</returns>
        int GetNextShipmentNumber(Guid storeSettingKey, Func<int> validate, int shipmentsCount = 1);

        /// <summary>
        /// Gets the complete collection of registered type fields
        /// </summary>
        /// <returns>The collection of <see cref="TypeField"/></returns>
        IEnumerable<ITypeField> GetTypeFields();
    }
}