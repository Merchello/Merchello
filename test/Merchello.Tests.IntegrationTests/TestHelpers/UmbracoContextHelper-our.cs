using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Moq;
using Umbraco.Web;

/* *****************************************************
 *   http://our.umbraco.org/forum/developers/api-questions/37255-How-can-I-unit-test-a-class-inheriting-from-SurfaceController  
 * *****************************************************/

namespace Merchello.Tests.Base.Umbraco
{

    /// <summary>
    /// Helper class which can be used to create an <see cref="UmbracoContext"/> instance for unit testing of surface controllers.
    /// </summary>
    public static class UmbracoContextHelper
    {
        #region Methods

        /// <summary>
        /// Helper method which creates an <see cref="UmbracoContext"/> instance populated with a <see cref="HttpContextBase"/> instance created using
        /// the specified <see cref="MockRepository"/> instance.
        /// </summary>
        /// <param name="mockRepository">The mock repository to use to create the <see cref="HttpContext"/> for the created <see cref="UmbracoContext"/></param>
        /// <param name="requestUri">The destination URL of the request being mocked.</param>
        /// <param name="mockCookie">A mock cookie to pull from the HttpContext</param>
        /// <returns>An <see cref="UmbracoContext"/> instance which can be used for basic unit testing of surface controllers.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011", Justification = "Do not derive from base class as suggested because it is obselete")]
        public static UmbracoContext CreateContext(MockRepository mockRepository, Uri requestUri, HttpCookie mockCookie = null)
        {
            Mock<HttpContextBase> mockHttpContext = mockRepository.Create<HttpContextBase>();
            Mock<HttpRequestBase> mockHttpRequest = mockRepository.Create<HttpRequestBase>();

            mockHttpContext.SetupGet(p => p.Request).Returns(mockHttpRequest.Object);
            mockHttpRequest.SetupGet(p => p.Url).Returns(requestUri);

            if(mockCookie != null)
            {               
                mockHttpContext.SetupGet(p => p.Request.Cookies[mockCookie.Name]).Returns(() => mockCookie);
                mockHttpContext.Setup(p => p.Response.Cookies.Add(mockCookie));
            }
            return CreateContext(mockHttpContext.Object);
        }

        /// <summary>
        /// Helper method which sets the UmbracoContext property of the 
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContextBase"/> instance which the <see cref="UmbracoContext"/> instance wraps. The URL property must be set.</param>
        /// <returns>An <see cref="UmbracoContext"/> instance which can be used for basic unit testing of surface controllers.</returns>
        public static UmbracoContext CreateContext(HttpContextBase httpContext)
        {
            Type umbracoContextType = typeof(UmbracoContext);
            Assembly umbracoWebAssembly = umbracoContextType.Assembly;
            global::Umbraco.Core.ApplicationContext applicationContext = CreateApplicationContext();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type[] umbracoContextConstructorArgTypes = new Type[] 
                { 
                    typeof(HttpContextBase),
                    typeof(global::Umbraco.Core.ApplicationContext),
                    umbracoWebAssembly.GetType("Umbraco.Web.Routing.IRoutesCache")
                };
            ConstructorInfo umbracoContextConstructor = umbracoContextType.GetConstructor(bindingFlags, null, CallingConventions.Any, umbracoContextConstructorArgTypes, null);
            UmbracoContext umbracoContext = (UmbracoContext)umbracoContextConstructor.Invoke(new object[] { httpContext, applicationContext, null });

            SetRoutingContext(umbracoContext);

            return umbracoContext;
        }

        /// <summary>
        /// Helper method which sets the umbraco page ID.
        /// </summary>
        /// <param name="context">The <see cref="UmbracoContext"/> instance to set the pageid for</param>
        /// <param name="pageId">The pageId to assign to the Umbraco context.</param>
        public static void SetPageId(this UmbracoContext context, int? pageId)
        {
            HttpContextBase httpContext = context.HttpContext;

            httpContext.Items["pageID"] = pageId.ToString();
        }

        /// <summary>
        /// Creates an Umbraco.Web.Routing.RoutingContext" instance and assigns it to the 
        /// RoutingContext property of the UmbracoContext instance specified by umbracoContext.
        /// </summary>
        /// <param name="umbracoContext">The UmbracoContext instance to set the RoutingContext on.</param>
        private static void SetRoutingContext(UmbracoContext umbracoContext)
        {
            Type umbracoContextType = typeof(UmbracoContext);
            Assembly umbracoWebAssembly = umbracoContextType.Assembly;
            Type routingContextType = umbracoWebAssembly.GetType("Umbraco.Web.Routing.RoutingContext");
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type[] routingContextConstructorArgTypes = new Type[] 
                { 
                    typeof(UmbracoContext),
                    typeof(IEnumerable<>).MakeGenericType(umbracoWebAssembly.GetType("Umbraco.Web.Routing.IPublishedContentLookup")),
                    umbracoWebAssembly.GetType("Umbraco.Web.Routing.IDocumentLastChanceLookup"),
                    umbracoWebAssembly.GetType("Umbraco.Web.IPublishedContentStore"),
                    umbracoWebAssembly.GetType("Umbraco.Web.Routing.NiceUrlProvider")
                };
            ConstructorInfo routingContextConstructor = routingContextType.GetConstructor(bindingFlags, null, CallingConventions.Any, routingContextConstructorArgTypes, null);
            PropertyInfo setContextProperty = umbracoContextType.GetProperty("RoutingContext", bindingFlags);
            object routingContext = routingContextConstructor.Invoke(new object[] { umbracoContext, null, null, null, null });

            setContextProperty.SetValue(umbracoContext, routingContext, new object[] { });
        }

        /// <summary>
        /// Helper method which creates an ApplicationContext with very little setup.
        /// </summary>
        /// <returns>An ApplicationContext instance setup for unit testing.</returns>
        /// <remarks>
        /// <para>This should be the equivalent of BaseWebTest.Initialize() in the Umbraco codebase.</para>
        /// <code>
        /// ApplicationContext = new ApplicationContext() { IsReady = true };
        /// </code>
        /// </remarks>
        private static global::Umbraco.Core.ApplicationContext CreateApplicationContext()
        {
            Type applicationContextType = typeof(global::Umbraco.Core.ApplicationContext);
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ConstructorInfo constructor = applicationContextType.GetConstructor(bindingFlags, null, CallingConventions.Any, Type.EmptyTypes, null);
            PropertyInfo propertyInfo = applicationContextType.GetProperty("IsReady", bindingFlags);
            global::Umbraco.Core.ApplicationContext context = (global::Umbraco.Core.ApplicationContext)constructor.Invoke(null);

            propertyInfo.SetValue(context, true, null);

            return context;
        }

        #endregion
    }

}