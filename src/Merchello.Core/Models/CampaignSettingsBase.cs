namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The campaign base.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CampaignSettingsBase : Entity, ICampaignSettingsBase
    {
        #region fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<CampaignSettingsBase, string>(x => x.Name);

        /// <summary>
        /// The alias selector.
        /// </summary>
        private static readonly PropertyInfo AliasSelector = ExpressionHelper.GetPropertyInfo<CampaignSettingsBase, string>(x => x.Alias);

        /// <summary>
        /// The description selector.
        /// </summary>
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<CampaignSettingsBase, string>(x => x.Description);

        /// <summary>
        /// The active selector.
        /// </summary>
        private static readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<CampaignSettingsBase, bool>(x => x.Active);

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The alias.
        /// </summary>
        private string _alias;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The active.
        /// </summary>
        private bool _active;

        #endregion

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            } 

            set 
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _name = value;
                        return _name;
                    }, 
                    _name, 
                    NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        [DataMember]
        public string Alias
        {
            get
            {
                return _alias;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _alias = value;
                        return _alias;
                    },
                    _alias,
                    AliasSelector);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _description = value;
                        return _description;
                    },
                    _description,
                    DescriptionSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [DataMember]
        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _active = value;
                        return _active;
                    },
                    _active,
                    ActiveSelector);
            }
        }
    }
}