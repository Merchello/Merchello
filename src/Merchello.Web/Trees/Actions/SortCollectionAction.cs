namespace Merchello.Web.Trees.Actions
{
    using umbraco.interfaces;

    /// <summary>
    /// The sort collection action.
    /// </summary>
    public class SortCollectionAction : IAction
    {
         /// <summary>
        /// The local singleton instance.
        /// </summary>
        private static readonly SortCollectionAction LocalInstance = new SortCollectionAction();

        /// <summary>
        /// Prevents a default instance of the <see cref="SortCollectionAction"/> class from being created.
        /// </summary>
        private SortCollectionAction()
        {            
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static SortCollectionAction Instance
        {
            get { return LocalInstance; }
        }

        /// <summary>
        /// Gets the letter.
        /// </summary>
        public char Letter
        {
            get
            {
                return '(';
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

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return "sortCollections";
            }
        }

        /// <summary>
        /// Gets a value indicating whether show in notifier.
        /// </summary>
        public bool ShowInNotifier 
        { 
            get
            {
                return true;
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
                return "navigation-vertical";
            }
        } 
    }
}