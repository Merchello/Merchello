using System.Collections.ObjectModel;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessorArgumentCollection : KeyedCollection<string, string>
    {
        protected override string GetKeyForItem(string item)
        {
            return item;
        }
    }
}