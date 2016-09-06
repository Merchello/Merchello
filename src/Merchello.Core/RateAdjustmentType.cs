namespace Merchello.Core
{
    /// REFACTOR this should be moved out of the Core namespace

    /// <summary>
    /// Defines whether a rate adjustment should be a fixed numeric adjustment or calculated as a percentage
    /// </summary>
    public enum RateAdjustmentType
    {
        /// <summary>
        /// Represents a numeric rate adjustment
        /// </summary>
        Numeric = 1,

        /// <summary>
        /// Represents a percentage rate adjustment
        /// </summary>
        Percentage = 2
    }
}