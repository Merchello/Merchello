# Merchello

Merchello is a highly configurable, open source eCommerce package for [Umbraco CMS](http://www.umbraco.com) v7 ONLY. However, this project is no longer actively developed. 

If you are looking for an eCommerce solution for Umbraco v8 then I suggest you take a look at Vendr. You can follow them on Twitter here [@heyvendr](https://twitter.com/heyvendr) or signup on their website [getvendr.net](https://getvendr.net)

## Basic features

Merchello comes with the following core features, but it's also built so you can expand on it to your needs. Here's what you get right out of the box:

* Products with  options and variants
* Flat rate shipping provider (by weight or price) for a single warehouse that can be adjusted by country and region
* Cash, Paypal, Purchase & Braintree payment providers
* Both invoicing and orders, created with each purchase
* Multiple currencies
* Product Picker datatype for Umbraco that lets you connect your Merchello product with your Umbraco content.
* Extended Content on Products and Options (Similar to Nested Content) - Use existing Umbraco property editors on your products and options.
* Products & Sales Collections - Organise your products and sales efficiently
* Discounts
* Reports

## Downloading

Look on the releases tab

## Documentation / Website

Our documentation is a tad outdated, but it's all we have at the moment. We are always looking for help with the docs ;)

[https://merchello.readme.io/](https://merchello.readme.io/)

## Contribute

We would love and need your help. If you want to contribute to Merchello's core, the easiest way to get started is to fork the project on [GitHub](https://github.com/merchello/Merchello) and open <code>src/Merchello.sln</code> in Visual Studio. 

The Merchello.Web.UI.Client project is where the backoffice JavaScript files are, and if you are looking to update anything in there. Make sure you run the grunt tasks after your have made your changes. Please see the ReadMe.md in the Merchello.Web.UI.Client project root for more information.

Once you are done, just build and run. The project comes with a local SQLCE database with the Bazaar starter kit. The login for the back office is

admin
1234

We're excited to see what you do!

## Please report bugs you find!

If you don't want to dip your fingers into the core, one of the best ways you can contribute to Merchello is by letting us know if something is going wrong. This feedback is very useful to us - we can't catch them all! To view existing issues or submit one of your own, visit [https://github.com/merchello/merchello/issues](https://github.com/merchello/merchello/issues).
