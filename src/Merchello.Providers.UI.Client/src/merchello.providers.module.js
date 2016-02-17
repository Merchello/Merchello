(function() {
    angular.module('merchello.providers',
        [
            'merchello.models',
            'merchello.services',
            'merchello.providers.models',
            'merchello.providers.directives',
            'merchello.providers.resources'
        ]);

    angular.module('merchello.providers.models', []);
    angular.module('merchello.providers.directives', []);
    angular.module('merchello.providers.resources', ['merchello.providers.models']);
    angular.module('merchello.plugins').requires.push('merchello.providers');
}());
