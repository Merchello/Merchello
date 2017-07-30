namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// Defines a DTO object that has an extended data string (JSON or legacy XML).
    /// </summary>
    public interface IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the serialized extended data collection contents.
        /// </summary>
        string ExtendedData { get; set; } 
    }
}