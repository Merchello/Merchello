(function () {

    angular.module('merchello.plugins.ups', [
        'merchello.models',
        'merchello.resources'
    ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.ups');

}());