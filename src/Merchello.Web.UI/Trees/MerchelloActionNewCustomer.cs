using System;
using umbraco.interfaces;
using umbraco.BasePages;

namespace Merchello.Web.UI.Trees
{
    public class MerchelloActionNewCustomer : IAction
	{
		//create singleton
		private static readonly MerchelloActionNewCustomer InnerInstance = new MerchelloActionNewCustomer();


        private MerchelloActionNewCustomer() { }

        public static MerchelloActionNewCustomer Instance
		{
			get { return InnerInstance; }
		}

		#region IAction Members

		public char Letter
		{
			get
			{
				return 'C';
			}
		}

		public string JsFunctionName
		{
			get
			{
				return null;
			}
		}

		public string JsSource
		{
			get
			{
				return null;
			}
		}

		public string Alias
		{
			get
			{
				return "CreateCustomer";
			}
		}

		public string Icon
		{
			get
			{
                return "plus";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				return true;
			}
		}
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