# Merchello.Solo

This library is used to run Merchello outside of an Umbraco instance.  As a result a good deal of the 
code is comprised of Umbraco constructs that Merchello requires directly copied from the [UmbracoCMS](https://github.com/umbraco/Umbraco-CMS) repository.  This is done so that
we can minimize the need to rely on "borrowing" objects and classes and building them into Merchello.

This is accomplished by a series of Merchello specific interfaces (often themselves copies of the original Umbraco interfaces - if they exist) and adapters.


