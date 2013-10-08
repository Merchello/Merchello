using System;
using Merchello.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>
    /// </summary>
    public interface IServiceContext
    {

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        ICustomerService CustomerService { get; }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        IProductService ProductService { get; }
        
        /// <summary>
        /// Gets the <see cref="IProductVariantService"/>
        /// </summary>
        IProductVariantService ProductVariantService { get; }

        /// <summary>
        /// Gets the <see cref="CustomerItemRegisterService"/>
        /// </summary>
        ICustomerItemRegisterService CustomerItemRegisterService { get; }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        IInvoiceService InvoiceService { get; }

        /// <summary>
        /// Gets the <see cref="IShippingService"/>
        /// </summary>
        IShippingService ShippingService { get; }
        
        /// <summary>
        /// Gets the <see cref="IWarehouseService"/>
        /// </summary>
        IWarehouseService WarehouseService { get; }
    }
    
}
