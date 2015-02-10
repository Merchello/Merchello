// Bootstrap the Merchello angular module
(function() {
// NOTE: We do not want to include merchello.models in the merchello module to assert it is isolated from
// the umbraco module
angular.module('merchello', [
    'umbraco.filters',
	'umbraco.directives',
    'umbraco.services',
    'umbraco.mocks',
    'merchello.filters',
    'merchello.directives',
    'merchello.resources',
    'merchello.services',
    'merchello.mocks',
    'merchello.plugins',
]);
angular.module('merchello.models', []);
angular.module('merchello.filters', []);
angular.module('merchello.directives', []);
angular.module('merchello.resources', []);
angular.module('merchello.services', ['merchello.models']);
angular.module('merchello.mocks', ['merchello.models']);
angular.module('merchello.plugins', [])

//// Assert our dependencies
angular.module('umbraco.packages').requires.push('merchello');
}());

