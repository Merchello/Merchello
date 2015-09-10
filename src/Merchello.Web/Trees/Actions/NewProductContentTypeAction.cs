namespace Merchello.Web.Trees.Actions
{
    using umbraco.interfaces;

    /// <summary>
    /// The new product content type action.
    /// </summary>
    public class NewProductContentTypeAction : IAction
    {
        /// <summary>
        /// The local singleton instance.
        /// </summary>
        private static readonly NewProductContentTypeAction LocalInstance = new NewProductContentTypeAction();

        /// <summary>
        /// Prevents a default instance of the <see cref="NewProductContentTypeAction"/> class from being created.
        /// </summary>
        private NewProductContentTypeAction()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NewProductContentTypeAction Instance
        {
            get
            {
                return LocalInstance;
            }
        }

        /// <summary>
        /// Gets the letter.
        /// </summary>
        public char Letter
        {
            get
            {
                return 'C';
            }
        }

        /// <summary>
        /// Gets a value indicating whether show in notifier.
        /// </summary>
        public bool ShowInNotifier
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether can be permission assigned.
        /// </summary>
        public bool CanBePermissionAssigned
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public string Icon
        {
            get
            {
                return "add";
            }
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return "createMerchProductContentType";
            }
        }

        /// <summary>
        /// Gets the JS function name.
        /// </summary>
        public string JsFunctionName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the JS source.
        /// </summary>
        public string JsSource
        {
            get
            {
                return string.Empty;
            }
        }
    }
}