namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The customer.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Customer : CustomerBase, ICustomer
    {
        #region fields

        /// <summary>
        /// The login name selector.
        /// </summary>
        private static readonly PropertyInfo LoginNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LoginName);

        /// <summary>
        /// The first name selector.
        /// </summary>
        private static readonly PropertyInfo FirstNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.FirstName);

        /// <summary>
        /// The last name selector.
        /// </summary>
        private static readonly PropertyInfo LastNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LastName);

        /// <summary>
        /// The email selector.
        /// </summary>
        private static readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.Email);

        /// <summary>
        /// The tax exempt selector.
        /// </summary>
        private static readonly PropertyInfo TaxExemptSelector = ExpressionHelper.GetPropertyInfo<Customer, bool>(x => x.TaxExempt);

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
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="loginName">
        /// The login Name associated with the membership provider users
        /// </param>
        internal Customer(string loginName) : base(false)
        {
            Mandate.ParameterNotNullOrEmpty(loginName, "loginName");

            _loginName = loginName;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        [IgnoreDataMember]
        public string FullName
        {
            get { return string.Format("{0} {1}", _firstName, _lastName).Trim(); }
        }

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [DataMember]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _firstName = value;
                        return _firstName;
                    }, 
                    _firstName, 
                    FirstNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [DataMember]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _lastName = value;
                        return _lastName;
                    }, 
                    _lastName, 
                    LastNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the email address of the customer
        /// </summary>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o => 
                    {
                        _email = value;
                        return _email;
                    }, 
                    _email, 
                    EmailSelector);                    
            }
        }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        [DataMember]
        public string LoginName
        {
            get
            {
                return _loginName;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _loginName = value;
                        return _loginName;
                    },
                    _loginName,
                    LoginNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt.
        /// </summary>
        [DataMember]
        public bool TaxExempt
        {
            get
            {
                return _taxExempt;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _taxExempt = value;
                        return _taxExempt;
                    },
                    _taxExempt,
                    TaxExemptSelector);
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
    }
}