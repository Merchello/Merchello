namespace Merchello.Core.Configuration.BackOffice
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a dashboard tree node link with a key.
    /// </summary>
    public interface IDashboardTreeNodeKeyLink : IDashboardTreeNodeLink, IHasKeyId
    {
    }
}