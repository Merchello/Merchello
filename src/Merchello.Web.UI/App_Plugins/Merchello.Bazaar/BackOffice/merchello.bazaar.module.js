angular.module('merchello.bazaar', [
    'umbraco.services'
]);
angular.module('merchello.plugins').requires.push('merchello.bazaar');
