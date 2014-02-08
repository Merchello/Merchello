using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using System.Web;
using Umbraco.Core;
using Umbraco.Web;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    internal class UmbracoContextHelper
    {
        public static UmbracoContext GetMockUmbracoContext(Uri requestUri, HttpCookie mockCookie = null)
        {
            
            //var mockHttpContext = MockRepository.GenerateStub<HttpContextBase>();            
            //var mockHttpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            //var mockHttpResponse = MockRepository.GenerateStub<HttpResponseBase>();
            //SetupResult.For(mockHttpContext.Request).Return(mockHttpRequest);
            //SetupResult.For(mockHttpRequest.Url).Return(requestUri);
            //SetupResult.For(mockHttpRequest.Cookies).Return(cookieCollection);

            //SetupResult.For(mockHttpContext.Response).Return(mockHttpResponse);
            //SetupResult.For(mockHttpResponse.Cookies).ReturncookieCollection);

            var umbracoContext = MockRepository.GenerateStrictMock<UmbracoContext>();

            var cookieCollection = new HttpCookieCollection();
            SetupResult.For(umbracoContext.HttpContext.Request.Cookies).Return(cookieCollection);

            if (mockCookie != null)
            {
                cookieCollection.Add(mockCookie);
                umbracoContext.Expect(c => c.HttpContext.Response.Cookies.Add(mockCookie));
            }
            SetupResult.For(umbracoContext.HttpContext.Request.Url).Return(requestUri);
            
            return umbracoContext;
        }
    }
}
