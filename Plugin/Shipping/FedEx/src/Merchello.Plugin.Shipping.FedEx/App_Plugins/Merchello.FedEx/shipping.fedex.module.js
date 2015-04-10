(function () {

    angular.module('merchello.plugins.fedex', [
        'merchello.models',
        'merchello.resources'
    ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.fedex');

}());