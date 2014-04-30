using System;
using System.Reflection;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    internal class Notification : Entity, INotification
    {
        private string _name;
        private string _description;
        private string _src;
        private Guid _statusKey;
        private string _recipients;
        private bool _sendToCustomer;
        private bool _disabled;

        private static readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<Notification, string>(x => x.Name);

        

        public string Name { get; set; }
        public string Description { get; set; }
        public string Src { get; set; }
        public Guid StatusKey { get; set; }
        public string Recipients { get; set; }
        public bool SendToCustomer { get; set; }
        public bool Disabled { get; set; }
    }
}