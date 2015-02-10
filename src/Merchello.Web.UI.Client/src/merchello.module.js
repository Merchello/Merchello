// Bootstrap the Merchello angular module
(function() {
    angular.module('merchello', [
        'umbraco.filters',
        'umbraco.directives',
        'umbraco.services',
        'merchello.filters',
        'merchello.directives',
        'merchello.plugins',
        'merchello.resources',
        'merchello.services'
    ]);
    angular.module('merchello.models', []);
    angular.module('merchello.filters', []);
    angular.module('merchello.directives', []);
    angular.module('merchello.resources', []);
    angular.module('merchello.services', ['merchello.models']);
    angular.module('merchello.plugins', []);
    //// Assert our dependencies
    angular.module('umbraco.packages').requires.push('merchello');

}());

// LEGACY NAMESPACES - THESE SHOULD NO LONGER BY USED
(function (merchello, undefined) {

    // Global namespaces defined
    merchello.Models = {};
    merchello.Controllers = {};
    merchello.Directives = {};
    merchello.Filters = {};
    merchello.Services = {};
    merchello.Helpers = {};

}(window.merchello = window.merchello || {}));

