/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferProviderSelectionController
 * @function
 *
 * @description
 * The controller to handle offer provider selection
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.NewOfferProviderSelectionController',
    ['$scope', '$location', 'navigationService', 'marketingResource', 'offerProviderDisplayBuilder',
    function($scope, $location, navigationService, marketingResource, offerProviderDisplayBuilder) {
        
        $scope.loaded = false;
        $scope.offerProviders = [];

        // exposed methods
        $scope.setSelection = setSelection;

        function init() {
            loadOfferProviders();
        }

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                $scope.offerProviders = offerProviderDisplayBuilder.transform(providers);
                $scope.loaded = true;
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function setSelection(selectedProvider) {
            navigationService.hideNavigation();
            var view = selectedProvider.backOfficeTree.routePath.replace('{0}', 'create');
            $location.url(view, true);
        }

        // initialize the controller
        init();
}]);
