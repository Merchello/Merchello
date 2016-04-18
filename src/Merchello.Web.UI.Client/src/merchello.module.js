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
    angular.module('merchello.resources', ['merchello.models']);
    angular.module('merchello.services', ['merchello.models']);
    angular.module('merchello.plugins', ['chart.js', 'ui.codemirror']);
    //// Inject our dependencies
    angular.module('umbraco.packages').requires.push('merchello');

}());


