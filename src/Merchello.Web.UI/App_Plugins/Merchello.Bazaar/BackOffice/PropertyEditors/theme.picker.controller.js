angular.module('merchello.bazaar').controller('Merchello.Plugin.Bazaar.ThemePickerController',
    ['$scope', '$http', 'umbRequestHelper',
    function($scope, $http, umbRequestHelper)
    {
        var url = Umbraco.Sys.ServerVariables["merchello.bazaar"]["merchelloBazaarPropertyEditorsApiBaseUrl"] + "GetThemes";

        umbRequestHelper.resourcePromise(
            $http.get(url),
            'Failed to retrieve themes the Merchello Starter Kit')
            .then(function (data) {
                $scope.themes = data;
            });
    }]);