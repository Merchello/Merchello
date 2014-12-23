var app = angular.module('merchello', [
	'merchello.filters',
	'merchello.directives',
	'merchello.resources',
	'merchello.services',
	'merchello.mocks',
    'ngCookies'
]);

/* For Angular 1.2: we need to load in Routing seperately
	    'ngRoute'
*/