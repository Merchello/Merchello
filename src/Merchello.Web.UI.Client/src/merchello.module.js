// Bootstrap the Merchello angular module
(function() {
    angular.module('merchello', [
        'umbraco.filters',
        'umbraco.directives',
        'umbraco.services',
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

    //// Assert our dependencies
    if (!('umbraco.filters' in angular.module('umbraco.packages').requires)) {
        angular.module('umbraco.packages').requires.push('umbraco.filters');
    }
    if (!('umbraco.directives' in angular.module('umbraco.packages').requires)) {
        angular.module('umbraco.packages').requires.push('umbraco.directives');
    }
    if (!('umbraco.services' in angular.module('umbraco.packages').requires)) {
        angular.module('umbraco.packages').requires.push('umbraco.services');
    }

    angular.module('umbraco.packages').requires.push('merchello');
}());

