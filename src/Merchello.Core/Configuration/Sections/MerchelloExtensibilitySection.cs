namespace Merchello.Core.Configuration.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Configuration.Elements;

    /// <inheritdoc/>
    internal class MerchelloExtensibilitySection : MerchelloSection, IMerchelloExtensibilitySection
    {
        /// <inheritdoc/>
        IMvcSection IMerchelloExtensibilitySection.Mvc
        {
            get
            {
                return this.Mvc;
            }
        }

        /// <inheritdoc/>
        IBackOfficeSection IMerchelloExtensibilitySection.BackOffice
        {
            get
            {
                return this.BackOffice;
            }
        }

        /// <inheritdoc/>
        IDictionary<string, Type> IMerchelloExtensibilitySection.Pluggable
        {
            get
            {
                return this.Pluggable.CreateDictionary("object");
            }
        }

        /// <inheritdoc/>
        IDictionary<string, Type> IMerchelloExtensibilitySection.Strategies
        {
            get
            {
                return this.Strategies.CreateDictionary("strategy");
            }
        }

        /// <inheritdoc/>
        IDictionary<string, IEnumerable<Type>> IMerchelloExtensibilitySection.TaskChains
        {
            get
            {
                return this.TaskChains.ChainsDictionary;
            }
        }

        /// <inheritdoc/>
        ITypeFieldsSection IMerchelloExtensibilitySection.TypeFields
        {
            get
            {
                return this.TypeFields;
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("mvc", IsRequired = true)]
        internal MvcElement Mvc
        {
            get
            {
                return (MvcElement)this["mvc"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("backoffice", IsRequired = true)]
        internal BackOfficeElement BackOffice
        {
            get
            {
                return (BackOfficeElement)this["backoffice"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("pluggable", IsRequired = true)]
        internal GenericTypeDictionaryElement Pluggable
        {
            get
            {
                return (GenericTypeDictionaryElement)this["pluggable"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("strategies", IsRequired = true)]
        internal GenericTypeDictionaryElement Strategies
        {
            get
            {
                return (GenericTypeDictionaryElement)this["strategies"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("taskChains", IsRequired = true)]
        internal TaskChainsElement TaskChains
        {
            get
            {
                return (TaskChainsElement)this["taskChains"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("typeFields", IsRequired = false)]
        internal TypeFieldsElement TypeFields
        {
            get
            {
                return (TypeFieldsElement)this["typeFields"];
            }
        }
    }
}