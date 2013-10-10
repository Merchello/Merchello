using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class Customer : CustomerBase, ICustomer
    {
        private int? _memberId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private decimal _totalInvoiced;
        private readonly decimal _totalPayments;
        private readonly DateTime? _lastPaymentDate;

        public Customer(decimal totalInvoice, decimal totalPayments, DateTime? lastPaymentDate)
            :base(false)
        {
            _totalInvoiced = totalInvoice;
            _totalPayments = totalPayments;
            _lastPaymentDate = lastPaymentDate;
        }

        private static readonly PropertyInfo MemberIdSelector = ExpressionHelper.GetPropertyInfo<Customer, int?>(x => x.MemberId);
        private static readonly PropertyInfo FirstNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.FirstName);
        private static readonly PropertyInfo LastNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LastName);
        private static readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.Email);

        /// <summary>
        /// Gets or sets the memberId
        /// </summary>
        [DataMember]
        public int? MemberId
        {
            get { return _memberId; }
            set
            {                
                SetPropertyValueAndDetectChanges(o => 
                    {
                        _memberId = value;
                        return _memberId;
                    }, _memberId, MemberIdSelector);
            }
        }

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
            get { return _firstName;  }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                    {
                        _firstName = value;
                        return _firstName;
                    }, _firstName, FirstNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [DataMember]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                    {
                        _lastName = value;
                        return _lastName;
                    }, _lastName, LastNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the email address of the customer
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return _email; }
            set
            {
                SetPropertyValueAndDetectChanges(o => 
                    {
                        _email = value;
                        return _email;
                    }, _email, EmailSelector);                    
            }
        }

        /// <summary>
        /// Gets the total amount invoiced
        /// </summary>
        [DataMember]
        public decimal TotalInvoiced
        {
            get { return _totalInvoiced; }
            internal set
            {
                _totalInvoiced = value;
            }
        }

        /// <summary>
        /// Gets the total payments
        /// </summary>
        [DataMember]
        public decimal TotalPayments
        {
            get { return _totalPayments; }
        }

        /// <summary>
        /// Gets the last payment date
        /// </summary>
        [DataMember]
        public DateTime? LastPaymentDate
        {
            get { return _lastPaymentDate; }
        }
        
    }
}