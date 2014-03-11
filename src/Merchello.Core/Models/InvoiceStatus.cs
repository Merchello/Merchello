using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class InvoiceStatus : Entity, IInvoiceStatus
    {
        private string _name;
        private string _alias;
        private bool _reportable;
        private bool _active;
        private int _sortOrder;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Name);
        private static readonly PropertyInfo AliasSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Alias);
        private static readonly PropertyInfo ReportableSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Reportable);
        private static readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Active);
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, int>(x => x.SortOrder);

        /// <summary>
        /// The name of the invoice status
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// The alias of the invoice status
        /// </summary>
        [DataMember]
        public string Alias
        {
            get { return _alias; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _alias = value;
                    return _alias;
                }, _alias, AliasSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not to report on this invoice status
        /// </summary>
        [DataMember]
        public bool Reportable
        {
            get { return _reportable; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _reportable = value;
                    return _reportable;
                }, _reportable, ReportableSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not this invoice status is active
        /// </summary>
        [DataMember]
        public bool Active
        {
            get { return _active; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _active = value;
                    return _active;
                }, _active, ActiveSelector);
            }
        }

        /// <summary>
        /// The sort order of the invoice status
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sortOrder = value;
                    return _sortOrder;
                }, _sortOrder, SortOrderSelector);
            }
        }
    }

}