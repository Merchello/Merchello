namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Merchello.Core.Acquired.Configuration;

    /// <summary>
    /// Responsible for parsing the taskChains configuration section
    /// </summary>
    internal class TaskChainsElement : RawXmlConfigurationElement
    {
        /// <summary>
        /// Gets a dictionary with key of the task chain alias and a collection of task types.
        /// </summary>
        public IDictionary<string, IEnumerable<Type>> ChainsDictionary
        {
            get
            {
                var d = new Dictionary<string, IEnumerable<Type>>();

                var chains = RawXml.Elements("taskChain");
                foreach (var chain in chains)
                {
                    var key = chain.Attribute("alias").Value;
                    var xtasks = chain.Element("tasks");
                    if (xtasks == null) throw new NullReferenceException("chainTask does not contain a tasks element");

                    var tasks = xtasks.Elements("task");

                    d.Add(key, Build(tasks));
                }


                return d;
            }
        }

        /// <summary>
        /// Gets the types of the tasks.
        /// </summary>
        /// <param name="xtasks">
        /// The list of type in the configuration file.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        private IEnumerable<Type> Build(IEnumerable<XElement> xtasks)
        {
            return xtasks.Select(x => Type.GetType(x.Attribute("type").Value)).ToArray();
        }
    }
}