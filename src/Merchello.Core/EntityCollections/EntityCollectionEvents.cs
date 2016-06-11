namespace Merchello.Core.EntityCollections
{
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// The entity collection events.
    /// </summary>
    public class EntityCollectionEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco Application Started handler.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        /// <remarks>
        /// Merchello is boot strapped in Application Starting so the GatewayProviderResolver should be good to go at this point.
        /// </remarks>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            EntityCollectionService.Created += EntityCollectionServiceOnCreated;
            EntityCollectionService.Saved += EntityCollectionServiceOnSaved;
            EntityCollectionService.Deleted += EntityCollectionServiceOnDeleted;
        }

        /// <summary>
        /// The entity collection service on saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void EntityCollectionServiceOnSaved(IEntityCollectionService sender, SaveEventArgs<IEntityCollection> e)
        {
            if (!EntityCollectionProviderResolver.HasCurrent) return;
            foreach (var collection in e.SavedEntities)
            {
                EntityCollectionProviderResolver.Current.AddOrUpdateCache(collection);
            }
        }

        /// <summary>
        /// The entity collection service on deleted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void EntityCollectionServiceOnDeleted(IEntityCollectionService sender, DeleteEventArgs<IEntityCollection> e)
        {
            if (!EntityCollectionProviderResolver.HasCurrent) return;
            foreach (var collection in e.DeletedEntities)
            {
                EntityCollectionProviderResolver.Current.RemoveFromCache(collection.Key);
            }
        }

        /// <summary>
        /// The entity collection service on created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void EntityCollectionServiceOnCreated(IEntityCollectionService sender, Events.NewEventArgs<IEntityCollection> e)
        {
            if (!EntityCollectionProviderResolver.HasCurrent) return;            
            
            EntityCollectionProviderResolver.Current.AddOrUpdateCache(e.Entity);
        }
    }
}