using AutoMapper;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web.Models.MapperResolvers
{
    /// <summary>
    /// Resolves NotificationMessages associated with the <see cref="INotificationMethod"/>
    /// </summary>
    public class NotificationMessagesResolver : ValueResolver<INotificationMethod, NotificationMethodDisplay>
    {
        protected override NotificationMethodDisplay ResolveCore(INotificationMethod source)
        {
            throw new System.NotImplementedException();
        }
    }
}