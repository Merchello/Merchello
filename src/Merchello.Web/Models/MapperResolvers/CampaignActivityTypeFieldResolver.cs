namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The campaign activity type field resolver.
    /// </summary>
    internal class CampaignActivityTypeFieldResolver : ValueResolver<ICampaignActivitySettings, TypeField>
    {
        /// <summary>
        /// Resolves the campaign activity type field
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="TypeField"/>.
        /// </returns>
        protected override TypeField ResolveCore(ICampaignActivitySettings source)
        {
            return (TypeField)source.GetTypeField();
        }
    }
}