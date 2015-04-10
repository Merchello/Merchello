(function() {
    angular.module('merchello.plugins.braintree',
        [
            'merchello.models',
            'merchello.services'
        ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.braintree');
}());