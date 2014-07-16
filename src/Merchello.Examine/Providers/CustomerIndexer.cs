namespace Merchello.Examine.Providers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;    
    using Core.Models;
    using global::Examine;
    using global::Examine.LuceneEngine;
    using global::Examine.LuceneEngine.Config;

    /// <summary>
    /// The customer indexer.
    /// </summary>
    public class CustomerIndexer : BaseMerchelloIndexer
    {
        /// <summary>
        /// The index field policies.
        /// </summary>
        internal static readonly List<StaticField> IndexFieldPolicies = new List<StaticField>()
        {
            new StaticField("customerKey", FieldIndexTypes.ANALYZED, false, string.Empty),
            new StaticField("loginName", FieldIndexTypes.ANALYZED, true, string.Empty),
            new StaticField("firstName", FieldIndexTypes.ANALYZED, true, string.Empty),
            new StaticField("lastName", FieldIndexTypes.ANALYZED, true, string.Empty),
            new StaticField("email", FieldIndexTypes.ANALYZED, true, string.Empty),
            new StaticField("taxExempt", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
            new StaticField("extendedData", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
            new StaticField("notes", FieldIndexTypes.ANALYZED, false, string.Empty),
            new StaticField("addresses", FieldIndexTypes.ANALYZED, false, string.Empty),
            new StaticField("lastActivityDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
            new StaticField("createDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
            new StaticField("updateDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME")
        };

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        protected override IEnumerable<string> SupportedTypes
        {
            get
            {
                return new[] { IndexTypes.Customer };
            }
        }


        /// <summary>
        /// Rebuilds the customer index
        /// </summary>
        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding the customer index");

            EnsureIndex(true);

            PerformIndexAll(IndexTypes.Customer);
        }

        /// <summary>
        /// Adds the customer to the index
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to be indexed</param>
        /// <remarks>For testing</remarks>
        internal void AddCustomerToIndex(ICustomer customer)
        {
            var nodes = new List<XElement> { customer.SerializeToXml().Root };
            AddNodesToIndex(nodes, IndexTypes.Customer);
        }

        /// <summary>
        /// Removes the customer from the index
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to be removed from the index</param>
        /// <remarks>For testing</remarks>
        internal void DeleteCustomerFromIndex(ICustomer customer)
        {
            DeleteFromIndex(((Customer)customer).ExamineId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The perform index all.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        protected override void PerformIndexAll(string type)
        {
            if (!SupportedTypes.Contains(type)) return;

            var customers = DataService.CustomerDataService.GetAll();

            var customersArray = customers as ICustomer[] ?? customers.ToArray();

            if (!customersArray.Any()) return;
            var nodes = customersArray.Select(i => i.SerializeToXml().Root).ToList();

            AddNodesToIndex(nodes, IndexTypes.Customer);
        }

        /// <summary>
        /// Creates an IIndexCriteria object based on the indexSet passed in and our DataService
        /// </summary>
        /// <param name="indexSet">
        /// The index Set.
        /// </param>
        /// <remarks>
        /// If we cannot initialize we will pass back empty indexer data since we cannot read from the database
        /// </remarks>
        /// <returns>
        /// The <see cref="IIndexCriteria"/>.
        /// </returns>
        protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        {
            return indexSet.ToIndexCriteria(DataService.CustomerDataService.GetIndexFieldNames(), IndexFieldPolicies);
        }

        /// <summary>
        /// return the index policy for the field name passed in, if not found, return normal
        /// </summary>
        /// <param name="fieldName">
        /// The field Name.
        /// </param>
        /// <returns>
        /// The <see cref="FieldIndexTypes"/>.
        /// </returns>
        protected override FieldIndexTypes GetPolicy(string fieldName)
        {
            var def = IndexFieldPolicies.Where(x => x.Name == fieldName).ToArray();
            return def.Any() == false ? FieldIndexTypes.ANALYZED : def.Single().IndexType;
        }
    }
}