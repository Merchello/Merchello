namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// The offer component attribute value resolver.
    /// </summary>
    internal class OfferComponentAttributeValueResolver : ValueResolver<OfferComponentBase, object>
    {
        /// <summary>
        /// The _field name.
        /// </summary>
        private readonly string _fieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentAttributeValueResolver"/> class.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        public OfferComponentAttributeValueResolver(string fieldName)
        {
            _fieldName = fieldName;
        }

        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object ResolveCore(OfferComponentBase source)
        {
            var att = source.GetType().GetCustomAttribute<OfferComponentAttribute>(false);
            if (att == null) return null;
            switch (_fieldName)
            {
                case "name":
                    return att.Name;
                case "key":
                    return att.Key;
                case "description":
                    return att.Description;
                case "editorView":
                    return new DialogEditorViewDisplay()
                    {
                        Title = att.Name,
                        Description = att.Description,
                        EditorView = att.EditorView.StartsWith("~/") ? IOHelper.ResolveUrl(att.EditorView) : att.EditorView
                    };
                case "restrictToType":
                    return att.RestrictToType == null ? string.Empty : att.RestrictToType.Name;
            }

            return null;
        }
    }
}