using System;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class Customer : MerchelloEntity, ICustomer
    {
        private int? _memberId;
        private string _firstName;
        private string _lastName;
        private readonly decimal _totalInvoiced;
        private readonly decimal _totalPayments;
        private readonly DateTime? _lastPaymentDate;

        public Customer(decimal totalInvoice, decimal totalPayments, DateTime? lastPaymentDate)
        {
            _totalInvoiced = totalInvoice;
            _totalPayments = totalPayments;
            _lastPaymentDate = lastPaymentDate;
        }

        private static readonly PropertyInfo MemberIdSelector = ExpressionHelper.GetPropertyInfo<Customer, int?>(x => x.MemberId);
        private static readonly PropertyInfo FirstNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.FirstName);
        private static readonly PropertyInfo LastNameSelector = ExpressionHelper.GetPropertyInfo<Customer, string>(x => x.LastName);


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
        /// Gets the total amount invoiced
        /// </summary>
        [DataMember]
        public decimal TotalInvoiced
        {
            get { return _totalInvoiced; }
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