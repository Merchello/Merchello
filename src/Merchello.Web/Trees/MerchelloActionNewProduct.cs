using System;
using umbraco.interfaces;
using umbraco.BasePages;

namespace Merchello.Web.Trees
{
    /// <summary>
    /// Fly out menu for the Catalog tree item to allow quick creation of a new product
    /// </summary>
    public class MerchelloActionNewProduct : IAction
	{
		//create singleton
		private static readonly MerchelloActionNewProduct InnerInstance = new MerchelloActionNewProduct();


        private MerchelloActionNewProduct() { }

        /// <summary>
        /// 
        /// </summary>
        public static MerchelloActionNewProduct Instance
		{
			get { return InnerInstance; }
		}

		#region IAction Members

        /// <summary>
        /// 
        /// </summary>
		public char Letter
		{
			get
			{
				return 'C';
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public string JsFunctionName
		{
			get
			{
				return null;
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public string JsSource
		{
			get
			{
				return null;
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public string Alias
		{
			get
			{
                return "ProductCreateActionMenu";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public string Icon
		{
			get
			{
                return "plus";
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public bool ShowInNotifier
		{
			get
			{
				return true;
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public bool CanBePermissionAssigned
		{
			get
			{
				return true;
			}
		}

		#endregion
	}
}