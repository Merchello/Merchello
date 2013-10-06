using System.Collections.ObjectModel;

namespace Merchello.Tests.Base.Prototyping
{
    public class OptionCollection : KeyedCollection<int, IOption>
    {
        protected override int GetKeyForItem(IOption item)
        {
            return item.Id;
        }
    }
}