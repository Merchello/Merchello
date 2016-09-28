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
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Alias
        {
            get
            {
                return _alias;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _alias, _ps.Value.AliasSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Reportable
        {
            get
            {
                return _reportable;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _reportable, _ps.Value.ReportableSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _active, _ps.Value.ActiveSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _sortOrder, _ps.Value.SortOrderSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Name);

            /// <summary>
            /// The alias selector.
            /// </summary>
            public readonly PropertyInfo AliasSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, string>(x => x.Alias);

            /// <summary>
            /// The reportable selector.
            /// </summary>
            public readonly PropertyInfo ReportableSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Reportable);

            /// <summary>
            /// The active selector.
            /// </summary>
            public readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, bool>(x => x.Active);

            /// <summary>
            /// The sort order selector.
            /// </summary>
            public readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<InvoiceStatus, int>(x => x.SortOrder);
        }
    }
}