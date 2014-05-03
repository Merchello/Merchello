namespace Merchello.Core.Triggers.Notification
{
    internal abstract class NotificationTriggerBase : TriggerBase
    {
        protected NotificationTriggerBase(IMerchelloContext merchelloContext) 
            : base(merchelloContext)
        { }
    }
}