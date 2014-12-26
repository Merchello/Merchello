// defines the Merchello Models namespace
if (typeof Merchello == 'undefined') var Merchello = {};
if (!Merchello.Models) Merchello.Models = {};

// the Merchello angular module
var merch = angular.module('merchello', [
    'umbraco.filters',
	'umbraco.directives',
	'umbraco.resources',
	'umbraco.services',
	'umbraco.packages',
    'ngCookies',
    'merchello.filters',
    'merchello.directives',
    'merchello.resources',
    'merchello.services',
    'merchello.mocks'
]);

// this is defined in the Umbraco app.js
// TODO it looks like we should be using the 'umbraco.packages' to store the Merchello module
// have requested clarification as to how this works
//var packages = packages || {};
//packages.requires = packages.requires || [];

angular.module('umbraco').requires.push(merch);