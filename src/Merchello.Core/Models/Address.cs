using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class Address : IdEntity, IAddress
    {
        private int _id;
        private Guid _customerPk;
        private string _label;
        private string _fullName;
        private string _company;
        private Guid _addressTypeFieldKey;
        private string _address1;
        private string _address2;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _countryCode;
        private string _phone;

        public Address(int id, Guid customerPk, string label)
        {
            _id = id;
            _customerPk = customerPk;
            _label = label;
        }

        private static readonly PropertyInfo IdSelector = ExpressionHelper.GetPropertyInfo<Address, int>(x => x.Id);
        private static readonly PropertyInfo CustomerPkSelector = ExpressionHelper.GetPropertyInfo<Address, Guid>(x => x.CustomerPk);
        private static readonly PropertyInfo LabelSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Label);


        public Guid CustomerPk
        {
            get { return _customerPk; }
            set { _customerPk = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        public Guid AddressTypeFieldKey
        {
            get { return _addressTypeFieldKey; }
            set { _addressTypeFieldKey = value; }
        }

        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }

        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        public string Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public string CountryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        #region Strongly-typed properties

        #endregion


        /// <summary>
        /// Method to call when EntityEntity is being saved
        /// </summary>
        /// <remarks>Created date is set and a Unique key is assigned</remarks>
        internal override void AddingEntity()
        {
            base.AddingEntity();

            if (Key == Guid.Empty)
                Key = Guid.NewGuid();
        }


    }
}