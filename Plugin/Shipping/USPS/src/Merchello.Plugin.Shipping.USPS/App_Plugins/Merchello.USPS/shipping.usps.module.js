(function () {

    angular.module('merchello.plugins.usps', [
        'merchello.models',
        'merchello.resources'
    ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.usps');

}());