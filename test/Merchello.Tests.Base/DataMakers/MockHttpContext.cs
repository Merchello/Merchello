using System;
using System.IO;
using System.Security.Principal;
using System.Web;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockHttpContext
    {

        public static HttpContext MockContext(string username = "")
        {
            var context = new HttpContext(
            new HttpRequest("", "http://tempuri.org", ""),
            new HttpResponse(new StringWriter())
            );

            if (string.IsNullOrEmpty(username)) return context;

            // User is logged in
            context.User = new GenericPrincipal(
                new GenericIdentity("username"),
                new string[0]
                );

            // User is logged out
            context.User = new GenericPrincipal(
                new GenericIdentity(String.Empty),
                new string[0]
                );
            return context;
        }

    }
}
