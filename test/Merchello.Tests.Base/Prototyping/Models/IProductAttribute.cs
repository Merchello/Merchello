using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping
{
    public interface IProductAttribute : IIdEntity
    {
        /// <summary>
        /// The id of the option which defines the attribute group this attribute belongs to
        /// </summary>
        int OptionId { get; }

        /// <summary>
        /// The name of the attribute
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The sort order for the product attribute with respect to an option
        /// </summary>
        int SortOrder { get; set; }
    }
}