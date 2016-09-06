# Merchello's Cache

We've copied several Umbraco internals to Merchello's Core so that we can reduce the
number of dependencies we have on Umbraco itself. 

From a usage standpoint, it is important to understand that the provider instances themselves
are not actually used while *Merchello is running within Umbraco*.  Instead Merchello implements a set of adapters
to map Umbraco's instantiated cache providers to Merchello's caching interfaces.

`ICacheProvider` and `IRuntimeCacheProvider` in Merchello are literally copies of the original Umbraco interfaces
with the same name e.g.

`Umbraco.Core.Cache.ICacheProvider` <- adaptor -> `Merchello.Core.Cache.ICacheProvider`

and 

`Umbraco.Core.Cache.IRuntimeCacheProvider` <- adaptor -> `Merchello.Core.Cache.IRuntimeCacheProvider`




 