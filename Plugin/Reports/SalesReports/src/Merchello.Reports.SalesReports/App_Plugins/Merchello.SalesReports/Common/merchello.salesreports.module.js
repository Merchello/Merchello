(function() {

    angular.module('merchello.salesreports', [
        'merchello.models',
        'merchello.directives',
        'merchello.resources',
        'merchello.salesreports.models'
    ]);

    angular.module('merchello.salesreports.models', ['merchello.models']);

    angular.module('merchello.plugins').requires.push('merchello.salesreports');
}());