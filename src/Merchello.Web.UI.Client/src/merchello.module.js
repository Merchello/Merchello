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
    'ngSanitize',
    'ngMobile',
    'blueimp.fileupload',
    'tmh.dynamicLocale',
    'merchello.filters',
    'merchello.directives',
    'merchello.resources',
    'merchello.services'
]);

// this is defined in the Umbraco app.js
var packages = packages || {};
packages.requires = packages.requires || [];
packages.requires.push(merch);
