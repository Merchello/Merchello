var app = angular.module('umbraco', [
	'umbraco.filters',
	'umbraco.directives',
	'umbraco.resources',
	'umbraco.services',
    'umbraco.packages',
	'umbraco.mocks',
	'umbraco.security',
    'ngCookies'
]);
var packages = angular.module("umbraco.packages", []);