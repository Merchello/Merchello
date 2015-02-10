(function() {

    angular.module('merchello.plugins.foa', [
        'merchello.models',
        'merchello.resources'
    ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.foa');

}());