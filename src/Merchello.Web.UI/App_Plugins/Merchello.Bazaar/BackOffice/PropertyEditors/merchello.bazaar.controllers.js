// MemberType Picker Controller
angular.module('merchello.bazaar').controller('Merchello.Plugin.Bazaar.MemberTypePickerController',
['$scope', '$http', 'umbRequestHelper',
    function ($scope, $http, umbRequestHelper) {

        var url = Umbraco.Sys.ServerVariables["merchelloBazaarUrls"]["merchelloBazaarPropertyEditorsApiBaseUrl"] + "GetMemberTypes";

        umbRequestHelper.resourcePromise(
            $http.get(url),
            'Failed to retrieve themes the Merchello Bazaar Starter Kit')
            .then(function (data) {
                $scope.memberTypes = data;
            });
    } 
]);

// Theme Picker Controller
angular.module('merchello.bazaar').controller('Merchello.Plugin.Bazaar.ThemePickerController',
    ['$scope', '$http', 'umbRequestHelper',
    function ($scope, $http, umbRequestHelper) {
        var url = Umbraco.Sys.ServerVariables["merchelloBazaarUrls"]["merchelloBazaarPropertyEditorsApiBaseUrl"] + "GetThemes";

        umbRequestHelper.resourcePromise(
            $http.get(url),
            'Failed to retrieve themes the Merchello Bazaar Starter Kit')
            .then(function (data) {
                $scope.themes = data;
            });
    }]);