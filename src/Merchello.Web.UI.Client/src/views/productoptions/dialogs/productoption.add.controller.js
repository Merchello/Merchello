angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionAddController',
    ['$scope', '$q', 'eventsService', 'merchelloTabsFactory', 'localizationService',
    function($scope, $q, eventsService, merchelloTabsFactory, localizationService) {

        $scope.tabs = [];

        $scope.visiblePanel = 'newoption';

        var newEvent = 'merchNewProductOptionSave';
        var sharedEvent = 'merchSharedProductOptionSave';

        if ($scope.dialogData.showTabs) {

            $scope.tabs = merchelloTabsFactory.createProductOptionAddTabs();

            $q.all([
                localizationService.localize('merchelloProductOptions_newOption'),
                localizationService.localize('merchelloProductOptions_sharedOption')
                ]).then(function(data) {

                $scope.tabs.addActionTab(
                    'newOption',
                    data[0],
                    function() {
                        $scope.tabs.setActive('newOption');
                        $scope.visiblePanel = 'newoption';
                    });

                $scope.tabs.addActionTab(
                    'sharedOption',
                    data[1],
                    function() {
                        $scope.tabs.setActive('sharedOption');
                        $scope.visiblePanel = 'sharedoption';
                    });

                $scope.tabs.setActive('newOption');
            });

        }


        $scope.validate = function() {

            var validation = { valid: false };

            if ($scope.visiblePanel === 'newoption') {
                eventsService.emit(newEvent, validation);
            } else if ($scope.visiblePanel === 'sharedoption') {
                eventsService.emit(sharedEvent, validation);
            }

            if (validation.valid) {
                $scope.submit($scope.dialogData);
            }
        }

}]);
