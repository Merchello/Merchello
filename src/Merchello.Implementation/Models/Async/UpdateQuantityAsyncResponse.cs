namespace Merchello.Implementation.Models.Async
{
    using System.Collections.Generic;

    public class UpdateQuantityAsyncResponse<TBasketLineItemModel> : AsyncResponse
        where TBasketLineItemModel : class, IBasketItemModel, new()
    {
        public UpdateQuantityAsyncResponse()
        {
            UpdatedItems = new List<TBasketLineItemModel>();
        }

        public string FormattedTotal { get; set; }

        public List<TBasketLineItemModel> UpdatedItems { get; set; }
    }
}