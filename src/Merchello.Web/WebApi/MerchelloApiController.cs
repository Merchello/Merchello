using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Services;

namespace Merchello.Web.WebApi
{
    public abstract class MerchelloApiController : ApiController
    {
        protected MerchelloApiController()
            : this(MerchelloContext.Current)
        {

        }

        protected MerchelloApiController(MerchelloContext merchelloContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }


        /// <summary>
        /// Returns the current MerchelloContext
        /// </summary>
        public MerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Useful for debugging
        /// </summary>
        internal Guid InstanceId { get; private set; }
    }
}
