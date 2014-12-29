// the Merchello angular module
var Merchello = {};
Merchello.Models = {};

(function() {
    var merch = angular.module('merchello', [
        'umbraco.filters',
        'umbraco.directives',
        'umbraco.resources',
        'umbraco.services',
        'umbraco.packages',
        'ngCookies',
        'merchello.models',
        'merchello.filters',
        'merchello.directives',
        'merchello.resources',
        'merchello.services',
        'merchello.mocks'
    ]);
    angular.module('merchello.models', []);
    angular.module('merchello.filters', []);
    angular.module('merchello.directives', []);
    angular.module('merchello.resources', []);
    angular.module('merchello.services', ['merchello.models', 'merchello.resources']);

    angular.module('umbraco.packages').requires.push('merchello');
}());

