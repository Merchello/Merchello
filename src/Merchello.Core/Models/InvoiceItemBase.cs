using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public abstract class InvoiceItemBase : Entity, IInvoiceItem
    {


        //private readonly static PropertyInfo PropertyCollectionSelector = ExpressionHelper.GetPropertyInfo<ContentBase, PropertyCollection>(x => x.Properties);

        //protected void PropertiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    OnPropertyChanged(PropertyCollectionSelector);
        //}


        public IEnumerable<IInvoiceItem> InvoiceItems { get; set; }

        public decimal Total()
        {
            throw new NotImplementedException();
        }
    }
}
