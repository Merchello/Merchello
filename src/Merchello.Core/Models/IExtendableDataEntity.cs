using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface IExtendableDataEntity
    {
        /// <summary>
        /// List of extended properties assoicated with this item
        /// </summary>
        //ExtendedDataCollection ExtendedProperties { get; set; }

        /// <summary>
        /// Indicates whether the object has extended datat with the supplied name
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>True if property exists, otherwise False</returns>
        bool HasProperty(string propertyName);

        /// <summary>
        /// Gets the value of an extended data property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Value as an <see cref="object"/></returns>
        object GetValue(string propertyName);

        /// <summary>
        /// Gets the value of the extended data property
        /// </summary>
        /// <typeparam name="TPassType">Type of the value to return</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Value as a <see cref="TPassType"/></returns>
        TPassType GetValue<TPassType>(string propertyName);

        /// <summary>
        /// SEts the <see cref="System.Object"/> value of an extended data property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="propertyValue">Value to set for the property</param>
        void SetValue(string propertyName, object propertyValue);
    }
}
