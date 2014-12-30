// Bootstrap the Merchello angular module
(function() {

angular.module('merchello', [
    'umbraco.filters',
	'umbraco.directives',
    'umbraco.services',
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

//// Assert our dependencies
var requires = angular.module('umbraco.packages').requires;
if ($.inArray('umbraco.filter', requires) < 0) {
    angular.module('umbraco.packages').requires.push('umbraco.filters');
}
if ($.inArray('umbraco.directives', requires) < 0) {
    angular.module('umbraco.packages').requires.push('umbraco.directives');
}
if ($.inArray('umbraco.services', requires) < 0) {
    angular.module('umbraco.packages').requires.push('umbraco.services');
}

angular.module('umbraco.packages').requires.push('merchello');
}());
