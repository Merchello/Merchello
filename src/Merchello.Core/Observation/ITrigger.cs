namespace Merchello.Core.Observation
{
    /// <summary>
    /// Marker interface for Observable triggers
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Method to call to "pull the trigger"
        /// </summary>
        /// <remarks>
        /// Update is a formal Observer pattern name
        /// </remarks>
        void Update();
    }
}