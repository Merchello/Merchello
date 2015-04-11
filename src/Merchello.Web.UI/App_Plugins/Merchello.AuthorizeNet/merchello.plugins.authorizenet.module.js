(function () {
    angular.module('merchello.plugins.authorizenet',
        [
            'merchello.models'
        ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.authorizenet');
}());