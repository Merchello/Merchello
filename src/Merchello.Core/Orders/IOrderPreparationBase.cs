using Merchello.Core.Builders;
using Merchello.Core.Models;

namespace Merchello.Core.Orders
{
    /// <summary>
    /// Defines an OrderPreparation
    /// </summary>
    public interface IOrderPreparationBase
    {

        IOrder PrepareOrder();

        IOrder PrepareOrder(IBuilderChain<IOrder> orderBuilder);


    }
}