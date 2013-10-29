using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Web.Models
{
    public class PublishedProduct //: IPublishedProduct
    {
      
    }

    public interface IPublishedProduct
    {
        Guid Key { get; }

        string Name { get; }


    }
}