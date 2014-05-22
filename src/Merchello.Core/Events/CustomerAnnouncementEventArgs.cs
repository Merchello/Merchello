using Merchello.Core.Models;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Customer announcement EventArgs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomerAnnouncementEventArgs<T> : AnnouncementEventArgs<T>
    {
        private readonly ICustomerBase _customer;

        public CustomerAnnouncementEventArgs(ICustomerBase customer, T eventObject)
            : this(customer,eventObject, true)
        {}

        public CustomerAnnouncementEventArgs(ICustomerBase customer, T eventObject, bool canCancel)
            : base(eventObject, canCancel)
        {
            _customer = customer;
        }

        /// <summary>
        /// The customer associated with the announcement
        /// </summary>
        public ICustomerBase Customer {
            get { return _customer; }
        }


    }
}