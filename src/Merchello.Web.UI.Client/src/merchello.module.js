// the Merchello angular module
(function() {
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
    angular.module('merchello.filters', []);
    angular.module('merchello.directives', []);
    angular.module('merchello.models', []);
    angular.module('merchello.resources', []);
    angular.module('merchello.services', ['merchello.models', 'merchello.resources']);

// this is defined in the Umbraco app.js
// TODO it looks like we should be using the 'umbraco.packages' to store the Merchello module
// have requested clarification as to how this works
//var packages = packages || {};
//packages.requires = packages.requires || [];

    angular.module('umbraco').requires.push('merchello');
}());

