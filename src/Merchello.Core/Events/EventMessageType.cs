namespace Merchello.Core.Events
{
    /// <summary>
    /// The type of event message
    /// </summary>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Events/MessageType.cs"/>
    public enum EventMessageType
    {
        Default = 0,
        Info = 1,
        Error = 2,
        Success = 3,
        Warning = 4
    }
}