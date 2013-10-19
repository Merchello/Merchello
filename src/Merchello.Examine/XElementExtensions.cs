using System.Xml.Linq;

namespace Merchello.Examine
{
    public static class XElementExtensions
    {
        public static void AddIdAttribute(this XElement el, int id)
        {
            el.SetAttributeValue("id", id);
        }
    }
}