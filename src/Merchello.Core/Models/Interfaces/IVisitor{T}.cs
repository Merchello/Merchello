namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// The Visitor interface.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type item that will be visited
    /// </typeparam>
    public interface IVisitor<TItem>
    {
        /// <summary>
        /// Executes the "visit"
        /// </summary>
        /// <param name="item">
        /// The item to be visited
        /// </param>
        /// <remarks>
        /// This is the Visitor design pattern.  PluralSight has some great intros
        /// </remarks>
        void Visit(TItem item);
    }
}