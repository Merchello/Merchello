(function() {
    angular.module('merchello.plugins.braintree',
        [
            'merchello.models'
        ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.braintree');
}());