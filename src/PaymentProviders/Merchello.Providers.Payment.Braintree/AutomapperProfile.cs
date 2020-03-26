namespace Merchello.Providers.Payment.Braintree
{
	using global::Braintree;
	using Merchello.Providers.Payment.Braintree.Models;
	public class AutomapperProfile : AutoMapper.Profile
	{
		protected override void Configure()
		{
			CreateMap<MerchantDescriptor, DescriptorRequest>();
		}
	}
}
