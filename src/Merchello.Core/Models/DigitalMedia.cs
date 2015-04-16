namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)


    /// <summary>
    /// The digital media.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class DigitalMedia : Entity, IDigitalMedia
    {
        #region Fields
        
        /// <summary>
        /// The name selector.
        /// </summary>
        /// <remarks>
        /// SR - This is used for the tracks dirty
        /// </remarks>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<DigitalMedia, string>(x => x.Name);


        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        #endregion


        /// <summary>
        /// Gets or sets the name.
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
    }
}