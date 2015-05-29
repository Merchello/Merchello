namespace Merchello.Web.Models.MapperResolvers.Offers
{
    using System;

    using AutoMapper;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Adds the value OfferExpires to the Display model based on offerStartDate and offerEndDate values
    /// </summary>
    public class OfferSettingsOfferExpiresResolver : ValueResolver<IOfferSettings, bool>
    {
        /// <summary>
        /// Override of AutoMapper ValueResolver ResolveCore method.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ResolveCore(IOfferSettings source)
        {
            return source.OfferStartsDate != DateTime.MinValue && source.OfferEndsDate != DateTime.MaxValue;
        }
    }
}