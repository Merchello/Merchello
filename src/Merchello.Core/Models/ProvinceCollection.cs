using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a collection of <see cref="IProvince"/>
    /// </summary>
    public class ProvinceCollection<T> :  NotifiyCollectionBase<string, T>
        where T : IProvince
    {
         private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

         protected override string GetKeyForItem(T item)
         {
             return item.Code;
         }

         internal new void Add(T item)
         {
             using (new WriteLock(_addLocker))
             {

                 if (Contains(item.Code)) return;
                 
                 base.Add(item);

                 OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
             }
         }


         public override int IndexOfKey(string code)
         {
             for (var i = 0; i < Count; i++)
             {
                 if (GetKeyForItem(this[i]) == code)
                 {
                     return i;
                 }
             }
             return -1;
         }

    }
}