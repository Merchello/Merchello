# Resolvers

Umbraco has a singleton name `Resolution` which it uses to "freeze" the setup / manipulation of it's resolvers
in Umbraco's boot managers.  The "frozen" check is done in the `ResolverBase` class (which all resolvers inherit from) by looking at
the `Resolution.IsFrozen` property.

This makes it difficult to control installs and migrations of Merchello granuarily as (if Merchello's resolvers are based on Umbraco's `ResolverBase`) as we 
must start Merchello's BootManager BEFORE Umbraco's boot sequence has completed to ensure that all of the resolvers have been setup.

In addition, it is more difficult to create the MerchelloContext in console applications (generally for product imports/exports) as it requires Umbraco to freeze it's `Resolution` singleton before
any of the resolvers can function.  

Looking for a better solution here, but due to the singleton, an adapter is not possible and there is quite a bit of functionality already written in
Merchello that relies on these resolvers.

This solution creates copies of all of the required resolvers, but `internal` as well as an `internal` `Resolution` singleton so that we can
perform Merchello's boot operations completely seperate from Umbraco allowing us to detect if database tables are present, schema version of the database (upgrade required),
and manage Merchello specific migrations and conceivably Merchello dependent migrations (like a starter kit update).
 