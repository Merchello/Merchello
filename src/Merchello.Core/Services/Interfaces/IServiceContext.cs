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

        ///// <summary>
        ///// Gets the <see cref="IAddressService"/>
        ///// </summary>
        //IAddressService AddressService { get; }

        ///// <summary>
        ///// Gets the <see cref="IAnonymousCustomerService"/>
        ///// </summary>
        //IAnonymousCustomerService AnonymousCustomerService { get; }

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        ICustomerService CustomerService { get; }

        /// <summary>
        /// Gets the <see cref="CustomerRegistryService"/>
        /// </summary>
        ICustomerRegistryService CustomerRegistryService { get; }


        ///IBasketItemService BasketItemService { get; }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        IInvoiceService InvoiceService { get; }

        ///// <summary>
        ///// Gets the <see cref="IInvoiceItemService"/>
        ///// </summary>
        //IInvoiceItemService InvoiceItemService { get;  }

        ///// <summary>
        ///// Gets the <see cref="IInvoiceStatusService"/>
        ///// </summary>
        //IInvoiceStatusService InvoiceStatusService { get; }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>
        /// </summary>
        IPaymentService PaymentService { get; }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        IProductService ProductService { get; }

        /// <summary>
        /// Gets the <see cref="IShipmentService"/>
        /// </summary>
        IShipmentService ShipmentService { get; }

        ///// <summary>
        ///// Gets the <see cref="IShipMethodService"/>
        ///// </summary>
        //IShipMethodService ShipMethodService { get; }

        ///// <summary>
        ///// Gets the <see cref="ITransactionService"/>
        ///// </summary>
        //ITransactionService TransactionService { get; }

        /// <summary>
        /// Gets the <see cref="IWarehouseService"/>
        /// </summary>
        IWarehouseService WarehouseService { get; }

        ///// <summary>
        ///// Gets the <see cref="IRegisteredGatewayProviderService"/>
        ///// </summary>
        //IRegisteredGatewayProviderService RegisteredGatewayProviderService { get; }
    }
    
}
