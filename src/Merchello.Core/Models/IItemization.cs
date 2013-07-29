using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Itemization
    /// </summary>
    public interface IItemization
    {

        decimal Total();
    }
}
