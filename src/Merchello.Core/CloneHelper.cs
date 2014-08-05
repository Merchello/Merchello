namespace Merchello.Core
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Helper methods for cloning objects
    /// </summary>
    internal class CloneHelper
    {
        /// <summary>
        /// Deep clone of an object
        /// </summary>
        /// <typeparam name="T">
        /// The type T of the object passed and returned
        /// </typeparam>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The a deep clone of object <see cref="T"/>.
        /// </returns>
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
