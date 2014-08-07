namespace Merchello.Core.Sales
{
    /// <summary>
    /// Utility class that allows for custom operations (edge case) to be performed on invoices, shipments, orders, payments
    /// </summary>
    public class SalesDocumentHelper
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesDocumentHelper"/> class.
        /// </summary>
        public SalesDocumentHelper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesDocumentHelper"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public SalesDocumentHelper(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            _merchelloContext = merchelloContext;
        }
    }
}