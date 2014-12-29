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
    /// The order indexer.
    /// </summary>
    public class OrderIndexer : BaseMerchelloIndexer
    {
        /// <summary>
        /// The index field policies.
        /// </summary>
        internal static readonly List<StaticField> IndexFieldPolicies = new List<StaticField>()
            {
                new StaticField("orderKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("invoiceKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("orderNumberPrefix", FieldIndexTypes.NOT_ANALYZED, true, string.Empty),
                new StaticField("orderNumber", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("prefixedOrderNumber", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("orderDate", FieldIndexTypes.ANALYZED, true, "DATETIME"),
                new StaticField("orderStatusKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("versionKey", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("exported", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("orderStatus", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("orderItems", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("createDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("updateDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("allDocs", FieldIndexTypes.ANALYZED, false, string.Empty)
            };

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        protected override IEnumerable<string> SupportedTypes
        {
            get { return new[] { IndexTypes.Order }; }
        }


        /// <summary>
        /// The rebuild index.
        /// </summary>
        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding the order index");

            EnsureIndex(true);

            PerformIndexAll(IndexTypes.Order);
        }

        /// <summary>
        /// Adds the order to the index
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <remarks>
        /// For testing
        /// </remarks>
        internal void AddOrderToIndex(IOrder order)
        {
            var nodes = new List<XElement> {order.SerializeToXml().Root};
            AddNodesToIndex(nodes, IndexTypes.Order);
        }

        /// <summary>
        /// Removes the order from the index
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <remarks>
        /// For testing
        /// </remarks>
        internal void DeleteOrderFromIndex(IOrder order)
        {            
            DeleteFromIndex(((Order)order).ExamineId.ToString(CultureInfo.InvariantCulture));
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
            return indexSet.ToIndexCriteria(DataService.OrderDataService.GetIndexFieldNames(), IndexFieldPolicies);
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

        /// <summary>
        /// The perform index all.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        protected override void PerformIndexAll(string type)
        {
            if (!SupportedTypes.Contains(type)) return;

            var orders = DataService.OrderDataService.GetAll();
            var ordersArray = orders as IOrder[] ?? orders.ToArray();

            if (!ordersArray.Any()) return;
            var nodes = ordersArray.Select(o => o.SerializeToXml().Root).ToList();

            AddNodesToIndex(nodes, IndexTypes.Order);
        }
    }
}