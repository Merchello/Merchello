namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Order and Invoice repositories must have unique order and invoice numbers.
    /// </summary>
    internal interface IAssertsMaxDocumentNumber
    {
        /// <summary>
        /// The get max document number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int GetMaxDocumentNumber();
    }
}