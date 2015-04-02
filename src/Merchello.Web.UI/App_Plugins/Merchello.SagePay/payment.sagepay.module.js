(function() {
    angular.module('merchello.plugins.sagepay', [
        'merchello.models'
    ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.sagepay');
}());