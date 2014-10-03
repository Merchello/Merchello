# Merchello.Plugin.Taxation.TaxJar

     // Author: Alex Lindgren
     // Twitter: @alexlindgren
     // Compatible: Merchello 1.4.1 and Umbraco 7.1.6

## About this package

This Merchello plugin uses the [TaxJar](http://www.taxjar.com/) [Smart Sales Tax API](http://www.taxjar.com/api/docs/#smart-sales-tax-api) to calculate state sales tax (for U.S. only).  You need a TaxJar account and API token.

## Installation and configuration

* Install as a local Umbraco package
* Activate the provider in the Gateway Providers section of Merchello Settings
* Edit the provider settings to update the API Token (which can be found in the API Access section of TaxJar's Account settings)
* In the Taxation section of Merchello settings, set the Tax Method to "Taxjar Provider" (for the United States only).
