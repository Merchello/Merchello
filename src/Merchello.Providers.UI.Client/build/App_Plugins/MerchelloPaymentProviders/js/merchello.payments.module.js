(function() {
    angular.module('merchello.payments',
        [
            'merchello.models',
            'merchello.services',
            'merchello.payments.models',
            'merchello.payments.directives',
            'merchello.payments.resources'
        ]);

    angular.module('merchello.payments.models', []);
    angular.module('merchello.payments.directives', []);
    angular.module('merchello.payments.resources', ['merchello.payments.models']);
    angular.module('merchello.plugins').requires.push('merchello.payments');
}());
