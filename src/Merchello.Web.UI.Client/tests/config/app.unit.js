var app = angular.module('umbraco', [
	'umbraco.filters',
	'umbraco.directives',
	'umbraco.resources',
	'umbraco.services',
    'umbraco.packages',
	'umbraco.mocks',
	'umbraco.security',
    'ngCookies',
	'tmh.dynamicLocale'
]);
var packages = angular.module("umbraco.packages", []);