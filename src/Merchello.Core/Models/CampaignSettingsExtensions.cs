namespace Merchello.Core.Models
{
    using System.Linq;

    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The campaign settings extensions.
    /// </summary>
    internal static class CampaignSettingsExtensions
    {
        /// <summary>
        /// Gets the <see cref="ITypeField"/> from the <see cref="ICampaignActivitySettings"/>.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        public static ITypeField GetTypeField(this ICampaignActivitySettings settings)
        {
            var type = EnumTypeFieldConverter.CampaignActivity.GetTypeField(settings.CampaignActivityTfKey);
            var typeField = EnumTypeFieldConverter.CampaignActivity.Discount;
            switch (type)
            {
                case CampaignActivityType.Custom:
                    typeField =
                        EnumTypeFieldConverter.CampaignActivity.CustomTypeFields.FirstOrDefault(
                            x => x.TypeKey.Equals(settings.CampaignActivityTfKey));
                    break;
                case CampaignActivityType.Discount:
                    typeField = EnumTypeFieldConverter.CampaignActivity.Discount;
                    break;
                case CampaignActivityType.Sale:
                    typeField = EnumTypeFieldConverter.CampaignActivity.Sale;
                    break;
            }

            return typeField;
        }
    }
}