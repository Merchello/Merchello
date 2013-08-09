namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// An interface that defines the object is tracking property changes and if it is Dirty
    /// </summary>
    public interface ITracksDirty
    {
        bool IsDirty();
        bool IsPropertyDirty(string propertyName);
        void ResetDirtyProperties();
    }
}
