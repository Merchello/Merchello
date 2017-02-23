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
        'merchello.services',
        'ngStorage'
    ]).config(['$sessionStorageProvider','$localStorageProvider',
        function ($sessionStorageProvider, $localStorageProvider) {
            $sessionStorageProvider.setKeyPrefix('merchello-');
            $localStorageProvider.setKeyPrefix('merchello-');
        }]);

    angular.module('merchello.models', []);
    angular.module('merchello.filters', []);
    angular.module('merchello.directives', []);
    angular.module('merchello.resources', ['merchello.models']);
    angular.module('merchello.services', ['merchello.models']);
    angular.module('merchello.plugins', ['chart.js']);
    //// Inject our dependencies
    angular.module('umbraco.packages').requires.push('merchello');

}());


