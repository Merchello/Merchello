namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The notified status base.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class NotifiedStatusBase : Entity, INotifyStatus
    {
        #region Fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Name);

        /// <summary>
        /// The alias selector.
        /// </summary>
        private static readonly PropertyInfo AliasSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Alias);

        /// <summary>
        /// The reportable selector.
        /// </summary>
        private static readonly PropertyInfo ReportableSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Reportable);

        /// <summary>
        /// The active selector.
        /// </summary>
        private static readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Active);

        /// <summary>
        /// The sort order selector.
        /// </summary>
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, int>(x => x.SortOrder);

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The alias.
        /// </summary>
        private string _alias;

        /// <summary>
        /// The reportable.
        /// </summary>
        private bool _reportable;

        /// <summary>
        /// The active.
        /// </summary>
        private bool _active;

        /// <summary>
        /// The sort order.
        /// </summary>
        private int _sortOrder;

        #endregion

        /// <summary>
        /// Gets or sets the name of the order status
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _name = value;
                    return _name;
                }, 
                _name, 
                NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the alias of the order status
        /// </summary>
        [DataMember]
        public string Alias
        {
            get
            {
                return _alias;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _alias = value;
                    return _alias;
                }, 
                _alias, 
                AliasSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to report on this order status
        /// </summary>
        [DataMember]
        public bool Reportable
        {
            get
            {
                return _reportable;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _reportable = value;
                    return _reportable;
                },
                _reportable, 
                ReportableSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this order status is active
        /// </summary>
        [DataMember]
        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _active = value;
                    return _active;
                }, 
                _active, 
                ActiveSelector);
            }
        }

        /// <summary>
        /// Gets or sets the sort order of the order status
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _sortOrder = value;
                    return _sortOrder;
                }, 
                _sortOrder, 
                SortOrderSelector);
            }
        }
    }
}