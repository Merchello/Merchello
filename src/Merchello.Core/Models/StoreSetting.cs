using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class StoreSetting : Entity, IStoreSetting
    {
        private string _name;
        private string _value;
        private string _typeName;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.Name);
        private static readonly PropertyInfo ValueSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.Value);
        private static readonly PropertyInfo TypeNameSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.TypeName);

        /// <summary>
        /// The name of the store setting
        /// </summary>
        /// <remarks>
        /// Should be unique but not enforced
        /// </remarks>
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
        /// The value of the store setting
        /// </summary>
        [DataMember]
        public string Value
        {
            get { return _value; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _value = value;
                    return _value;
                }, _value, ValueSelector);
            }
        }

        /// <summary>
        /// The type of the store setting
        /// </summary>
        [DataMember]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _typeName = value;
                    return _typeName;
                }, _typeName, TypeNameSelector);
            }
        }
    }
}