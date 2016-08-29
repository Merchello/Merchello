namespace Merchello.Web
{
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing.Collections;
    using Merchello.Web.Models.MapperResolvers;
    using Merchello.Web.Models.MapperResolvers.EntityCollections;

    /// <summary>
    /// The auto mapper mappings.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// The create entity collection mappings.
        /// </summary>
        private static void CreateEntityCollectionMappings()
        {
            // Entity Collection
            AutoMapper.Mapper.CreateMap<IEntityCollection, EntityCollectionDisplay>()
                .ForMember(
                    dest => dest.EntityTypeField,
                    opt =>
                    opt.ResolveUsing<EntityTypeFieldResolver>().ConstructedBy(() => new EntityTypeFieldResolver()))
                .ForMember(
                    dest => dest.ExtendedData,
                    opt =>
                    opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(
                    dest => dest.ParentKey,
                    opt =>
                        opt.ResolveUsing<EntityCollectionNullableParentKeyResolver>().ConstructedBy(() => new EntityCollectionNullableParentKeyResolver()));

            AutoMapper.Mapper.CreateMap<IEntityFilterGroup, EntityFilterGroupDisplay>()
                .ForMember(
                    dest => dest.EntityTypeField,
                    opt =>
                    opt.ResolveUsing<EntityTypeFieldResolver>().ConstructedBy(() => new EntityTypeFieldResolver()))
                .ForMember(
                    dest => dest.ParentKey,
                    opt =>
                        opt.ResolveUsing<EntityCollectionNullableParentKeyResolver>().ConstructedBy(() => new EntityCollectionNullableParentKeyResolver()))
                .ForMember(
                    dest => dest.ExtendedData,
                    opt =>
                    opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(
                     dest => dest.Filters,
                     opt =>
                        opt.ResolveUsing<EntityFilterGroupFiltersValueResolver>().ConstructedBy(() => new EntityFilterGroupFiltersValueResolver()));

            AutoMapper.Mapper.CreateMap<EntityCollectionProviderAttribute, EntityCollectionProviderDisplay>()
                .ForMember(
                    dest => dest.EntityTypeField,
                    opt =>
                    opt.ResolveUsing<EntityTypeFieldResolver>().ConstructedBy(() => new EntityTypeFieldResolver()))
                .ForMember(
                    dest => dest.ManagedCollections,
                    opt =>
                    opt.ResolveUsing<ManagedCollectionsResolver>().ConstructedBy(() => new ManagedCollectionsResolver()))
                .ForMember(
                    dest => dest.DialogEditorView,
                    opt =>
                    opt.ResolveUsing<EntityCollectionProviderDialogEditorViewResolver>().ConstructedBy(() => new EntityCollectionProviderDialogEditorViewResolver()));
        }
    }
}