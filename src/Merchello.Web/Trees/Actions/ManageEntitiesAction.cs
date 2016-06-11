namespace Merchello.Web.Trees.Actions
{
    using umbraco.interfaces;

    /// <summary>
    /// The manage entities action.
    /// </summary>
    public class ManageEntitiesAction : IAction
    {
        /// <summary>
        /// The local singleton instance.
        /// </summary>
        private static readonly ManageEntitiesAction LocalInstance = new ManageEntitiesAction();

        /// <summary>
        /// Prevents a default instance of the <see cref="ManageEntitiesAction"/> class from being created.
        /// </summary>
        private ManageEntitiesAction()
        {            
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ManageEntitiesAction Instance
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
                return '0';
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
                return "item-arrangement";
            } 
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return "manageCollection";
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