namespace Merchello.Web.Models.DetachedContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web.Models;

    internal class DetachedPublishedContent : PublishedContentBase
    {
        private readonly string _name;
        private readonly PublishedContentType _contentType;
        private readonly IEnumerable<IPublishedProperty> _properties;
        private readonly int _sortOrder;
        private readonly bool _isPreviewing;
        private readonly IPublishedContent _containerNode;

        public DetachedPublishedContent(string name,
            PublishedContentType contentType,
            IEnumerable<IPublishedProperty> properties,
            IPublishedContent containerNode = null,
            int sortOrder = 0,
            bool isPreviewing = false)
        {
            this._name = name;
            this._contentType = contentType;
            this._properties = properties;
            this._sortOrder = sortOrder;
            this._isPreviewing = isPreviewing;
            this._containerNode = containerNode;
        }

        public override int Id
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return this._name; }
        }

        public override bool IsDraft
        {
            get { return this._isPreviewing; }
        }

        public override PublishedItemType ItemType
        {
            get { return PublishedItemType.Content; }
        }

        public override PublishedContentType ContentType
        {
            get { return this._contentType; }
        }

        public override string DocumentTypeAlias
        {
            get { return this._contentType.Alias; }
        }

        public override int DocumentTypeId
        {
            get { return this._contentType.Id; }
        }

        public override ICollection<IPublishedProperty> Properties
        {
            get { return this._properties.ToArray(); }
        }

        public override IPublishedProperty GetProperty(string alias)
        {
            return this._properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        public override IPublishedProperty GetProperty(string alias, bool recurse)
        {
            if (recurse)
                throw new NotSupportedException();

            return this.GetProperty(alias);
        }

        public override IPublishedContent Parent
        {
            get { return null; }
        }

        public override IEnumerable<IPublishedContent> Children
        {
            get { return Enumerable.Empty<IPublishedContent>(); }
        }

        public override int TemplateId
        {
            get { return 0; }
        }

        public override int SortOrder
        {
            get { return this._sortOrder; }
        }

        public override string UrlName
        {
            get { return null; }
        }

        public override string WriterName
        {
            get { return this._containerNode != null ? this._containerNode.WriterName : null; }
        }

        public override string CreatorName
        {
            get { return this._containerNode != null ? this._containerNode.CreatorName : null; }
        }

        public override int WriterId
        {
            get { return this._containerNode != null ? this._containerNode.WriterId : 0; }
        }

        public override int CreatorId
        {
            get { return this._containerNode != null ? this._containerNode.CreatorId : 0; }
        }

        public override string Path
        {
            get { return null; }
        }

        public override DateTime CreateDate
        {
            get { return this._containerNode != null ? this._containerNode.CreateDate : DateTime.MinValue; }
        }

        public override DateTime UpdateDate
        {
            get { return this._containerNode != null ? this._containerNode.UpdateDate : DateTime.MinValue; }
        }

        public override Guid Version
        {
            get { return this._containerNode != null ? this._containerNode.Version : Guid.Empty; }
        }

        public override int Level
        {
            get { return 0; }
        }
    }
}