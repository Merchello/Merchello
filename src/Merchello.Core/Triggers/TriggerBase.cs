using System;
using Merchello.Core.Triggers.Notification;

namespace Merchello.Core.Triggers
{
    internal abstract class TriggerBase : IEventTrigger
    {
        private readonly IMerchelloContext _merchelloContext;

        protected TriggerBase(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
        }

        public abstract void Invoke(object sender, EventArgs e);


        protected IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
        }
    }
}