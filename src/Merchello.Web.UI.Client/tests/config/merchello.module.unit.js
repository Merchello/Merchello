// the Merchello angular module
// TODO this should be able to be isolated to 'merchello.models' and purposely injected where
// we need it
var Merchello = {};
Merchello.Models = {};
(function() {

angular.module('merchello', [
    'umbraco.filters',
	'umbraco.directives',
    'merchello.models',
    'merchello.filters',
    'merchello.directives',
    'merchello.resources',
    'merchello.services',
    'merchello.mocks'
]);
angular.module('merchello.models', []);
angular.module('merchello.filters', []);
angular.module('merchello.directives', []);
angular.module('merchello.resources', []);
angular.module('merchello.services', ['merchello.models', 'merchello.resources']);
angular.module('merchello.mocks', ['merchello.models']);

angular.module('umbraco.packages').requires.push('merchello');
}());
