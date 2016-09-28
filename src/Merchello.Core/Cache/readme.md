# Merchello's Cache

We've copied several Umbraco internals to Merchello's Core so that we can reduce the
number of dependencies we have on Umbraco itself. 

`ICacheProvider` and `IRuntimeCacheProvider` in Merchello are literally copies of the original Umbraco interfaces
with the same name e.g.

Merchello does not use Umbraco's actual caching providers

 1. They are internal 
 2. They rely on Umbraco's IEntity and Merchello defines it's own IEntity with different primary keys