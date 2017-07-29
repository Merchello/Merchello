namespace Merchello.Core.Models
{
    

    /// <summary>
    /// Represents a state, region or province reference.
    /// </summary>
    public interface IProvince
    {
        /// <summary>
        /// Gets the name of the province
        /// </summary>
        
        string Name { get; }

        /// <summary>
        /// Gets the two letter province code
        /// </summary>
        
        string Code { get; }
    }
}