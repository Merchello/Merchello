using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Merchello.Core
{
    /// <summary>
    /// Helper methods for cloning objects
    /// </summary>
    internal class CloneHelper
    {
        /// <summary>
        /// Deep clone of an object
        /// </summary>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
