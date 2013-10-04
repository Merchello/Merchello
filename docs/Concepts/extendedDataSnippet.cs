using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.UnitTests.ExtendedData
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ExtendedData
    {
        
    }

    internal class ExtendedDataTypeField : TypeFieldMapper<ExtendedDataType>, IExtendedDataTypeField
    {
        internal override void BuildCache()
        {
            throw new NotImplementedException();
        }

        protected override ITypeField GetCustom(string alias)
        {
            throw new NotImplementedException();
        }


        public ITypeField Customer {
            get { return GetTypeField(ExtendedDataType.Customer); }
        }
        public ITypeField ListItem { get { return GetTypeField(ExtendedDataType.ListItem); } }
        public ITypeField Transaction { get { return GetTypeField(ExtendedDataType.Transaction); } }
        public ITypeField RateQuote { get { return GetTypeField(ExtendedDataType.RateQuote); } }
    }

    internal interface IExtendedDataTypeField : ITypeFieldMapper<ExtendedDataType>
    {
        ITypeField Customer { get { return GetTypeField(ExtendedDataType.Customer); } }   

        ITypeField ListItem { get; }

        ITypeField Transaction { get; }

        ITypeField RateQuote { get; }
    }


    public enum ExtendedDataType
    {
        Customer,
        ListItem,
        Transaction,
        RateQuote,        
        Custom
    }

}


