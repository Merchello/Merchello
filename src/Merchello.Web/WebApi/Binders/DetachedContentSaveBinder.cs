namespace Merchello.Web.WebApi.Binders
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.ModelBinding;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.IO;
    using Umbraco.Web;
    using Umbraco.Web.Models.ContentEditing;
    using Umbraco.Web.Security;

    /// <summary>
    /// The detached content save binder.
    /// </summary>
    /// <typeparam name="TModelSave">
    /// The type of display model
    /// </typeparam>
    public abstract class DetachedContentSaveBinder<TModelSave> : IModelBinder
        where TModelSave : class, IHaveUploadedFiles
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentSaveBinder{TModelSave}"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected DetachedContentSaveBinder(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        protected ApplicationContext ApplicationContext { get; private set; }

        /// <summary>
        /// The bind model.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="bindingContext">
        /// The binding context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // NOTE: Validation is done in the filter
            if (actionContext.Request.Content.IsMimeMultipartContent() == false)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = IOHelper.MapPath("~/App_Data/TEMP/FileUploads");

            //// ensure it exists
            Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);

            var task = Task.Run(() => GetModelAsync(actionContext, bindingContext, provider))
                          .ContinueWith(x =>
                          {
                              if (x.IsFaulted && x.Exception != null)
                              {
                                  throw x.Exception;
                              }

                              // TODO validation
                              //now that everything is binded, validate the properties
                              //var contentItemValidator = GetValidationHelper();
                              //contentItemValidator.ValidateItem(actionContext, x.Result);

                              bindingContext.Model = x.Result;
                          });

            task.Wait();

            return bindingContext.Model != null;
        }


        /// <summary>
        /// Builds the model from the request contents
        /// </summary>
        /// <param name="actionContext">
        /// The action Context.
        /// </param>
        /// <param name="bindingContext">
        /// The binding Context.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<TModelSave> GetModelAsync(HttpActionContext actionContext, ModelBindingContext bindingContext, MultipartFormDataStreamProvider provider)
        {
            var request = actionContext.Request;

            // IMPORTANT!!! We need to ensure the umbraco context here because this is running in an async thread
            var httpContext = (HttpContextBase)request.Properties["MS_HttpContext"];
            UmbracoContext.EnsureContext(
                httpContext,
                ApplicationContext.Current,
                new WebSecurity(httpContext, ApplicationContext.Current));

            var content = request.Content;

            var result = await content.ReadAsMultipartAsync(provider);

            if (result.FormData["detachedContentItem"] == null)
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "The request was not formatted correctly and is missing the 'contentItem' parameter";
                throw new HttpResponseException(response);
            }

            // get the string json from the request
            var contentItem = result.FormData["detachedContentItem"];

            // deserialize into our model
            var model = JsonConvert.DeserializeObject<TModelSave>(contentItem);

            ////get the default body validator and validate the object
            var bodyValidator = actionContext.ControllerContext.Configuration.Services.GetBodyModelValidator();
            var metadataProvider = actionContext.ControllerContext.Configuration.Services.GetModelMetadataProvider();
            ////all validation errors will not contain a prefix
            bodyValidator.Validate(model, typeof(TModelSave), metadataProvider, actionContext, string.Empty);

            //// get the files
            foreach (var file in result.FileData)
            {
                //// The name that has been assigned in JS has 2 parts and the second part indicates the property id 
                //// for which the file belongs.
                var parts = file.Headers.ContentDisposition.Name.Trim(new char[] { '\"' }).Split('_');
                if (parts.Length != 2)
                {
                    var response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                    response.ReasonPhrase = "The request was not formatted correctly the file name's must be underscore delimited";
                    throw new HttpResponseException(response);
                }

                var propAlias = parts[1];

                var fileName = file.Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });

                model.UploadedFiles.Add(new ContentItemFile
                {
                    TempFilePath = file.LocalFileName,
                    PropertyAlias = propAlias,
                    FileName = fileName
                });
            }

            return model;
        }
    }
}