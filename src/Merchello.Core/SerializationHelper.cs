namespace Merchello.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Umbraco.Core;

    /// <summary>
    /// Utility class to help in entity serialization
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// Helper method to Serialize Xml using the DataContractSerializer
        /// </summary>
        /// <typeparam name="T">
        /// The type of the entity to serialize
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// An Xml string
        /// </returns>
        public static string SerializeToXml<T>(T entity)
        {            
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment
                };

                using (var xmlWriter = XmlWriter.Create(sw, settings))
                {
                    var serializer = new DataContractSerializer(typeof(T));      
          
                    serializer.WriteObject(xmlWriter, entity);                   
                }

                return sw.ToString();
            }
        }

        /// <summary>
        /// Helper method to Deserialize Xml using the DataContractSerializer
        /// </summary>
        /// <typeparam name="T">The type of entity to be deserialized</typeparam>
        /// <param name="xml">The xml string that represents the entity</param>
        /// <returns>
        /// An <see cref="Attempt{T}"/>.  A successful attempt with have an instantiated object T
        /// </returns>
        public static Attempt<T> DeserializeXml<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(sr))
                { 
                    var serializer = new DataContractSerializer(typeof(T));

                    try
                    {
                        return Attempt<T>.Succeed((T)serializer.ReadObject(xmlReader));
                    }
                    catch (Exception ex)
                    {
                        return Attempt<T>.Fail(ex);
                    }                    
                }
            }
        }         
    }
}