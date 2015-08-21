﻿namespace Merchello.Web
{
    using Merchello.Web.Models.ContentEditing.EmbeddedContent;
    using Merchello.Web.Models.MapperResolvers.DetachedContent;

    using Umbraco.Core.Models;

    /// <summary>
    /// The auto mapper mappings.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// The create detached content mappings.
        /// </summary>
        private static void CreateDetachedContentMappings()
        {
            AutoMapper.Mapper.CreateMap<IContentType, EmbeddedContentTypeDisplay>()
                .ForMember(
                    dest => dest.Tabs,
                    opt =>
                    opt.ResolveUsing<EmbeddedContentTabsResolver>()
                        .ConstructedBy(() => new EmbeddedContentTabsResolver()));
        }
    }
}