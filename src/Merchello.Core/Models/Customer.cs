namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The customer.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Customer : CustomerBase, ICustomer
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region fields

        /// <summary>
        /// The first name.
        /// </summary>
        private string _firstName;

        /// <summary>
        /// The last name.
        /// </summary>
        private string _lastName;

        /// <summary>
        /// The email.
        /// </summary>
        private string _email;

        /// <summary>
        /// The login name.
        /// </summary>
        private string _loginName;

        /// <summary>
        /// The tax exempt.
        /// </summary>
        private bool _taxExempt;

        /// <summary>
        /// The _notes.
        /// </summary>
        private IEnumerable<INote> _notes;

        /// <summary>
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        /// <summary>
        /// The addresses.
        /// </summary>
        private IEnumerable<ICustomerAddress> _addresses; 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="loginName">
        /// The login Name associated with the membership provider users
        /// </param>
        internal Customer(string loginName) : base(false)
        {
            Ensure.ParameterNotNullOrEmpty(loginName, "loginName");

            _loginName = loginName;

            _addresses = new List<ICustomerAddress>();
            _notes = new List<INote>();
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        [IgnoreDataMember]
        public string FullName
        {
            get { return string.Format("{0} {1}", _firstName, _lastName).Trim(); }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _firstName, _ps.Value.FirstNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _lastName, _ps.Value.LastNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _email, _ps.Value.EmailSelector);                    
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string LoginName
        {
            get
            {
                return _loginName;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _loginName, _ps.Value.LoginNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool TaxExempt
        {
            get
            {
                return _taxExempt;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _taxExempt, _ps.Value.TaxExemptSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public IEnumerable<INote> Notes
        {
            get
            {
                return _notes;
            }

            set
            {
                // REFACTOR Should be a notify collection
                _notes = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public IEnumerable<ICustomerAddress> Addresses
        {
            get
            {
                return _addresses;
            }

            internal set
            {
                // REFACTOR Should be a notify collection
                _addresses = value;
            }
        }

        /// <summary>
        /// Gets or sets the examine id.
        /// </summary>
        [IgnoreDataMember]
        internal int ExamineId
        {
            get { return _examineId; }
            set { _examineId = value; }
        }

        /// <summary>
        /// Property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The login name selector.
            /// </summary>
            public readonly PropertyInfo LoginNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LoginName);

            /// <summary>
            /// The first name selector.
            /// </summary>
            public readonly PropertyInfo FirstNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.FirstName);

            /// <summary>
            /// The last name selector.
            /// </summary>
            public readonly PropertyInfo LastNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LastName);

            /// <summary>
            /// The email selector.
            /// </summary>
            public readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.Email);

            /// <summary>
            /// The tax exempt selector.
            /// </summary>
            public readonly PropertyInfo TaxExemptSelector = ExpressionHelper.GetPropertyInfo<Customer, bool>(x => x.TaxExempt);

        }
    }
}