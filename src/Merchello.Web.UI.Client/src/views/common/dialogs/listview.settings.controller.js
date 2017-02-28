angular.module('merchello').controller('Merchello.Common.Dialogs.ListViewSettingsController',
    ['$scope', '$q', 'localizationService',
     function($scope, $q, localizationService) {

         $scope.loaded = false;

         $scope.stickListTabTitle = '';

         function init() {

             var tokenKey = '';
             switch ($scope.dialogData.entityType) {
                 case 'Invoice':
                     tokenKey = 'merchelloTabs_' + 'sales';
                     break;
                 case 'Customer':
                     tokenKey = 'merchelloTabs_' + 'customer';
                     break;
                 case 'Offer':
                     tokenKey = 'merchelloTabs_' + 'offer';
                     break;
                 default:
                     tokenKey = 'merchelloTabs_' + 'product';
                     break;
             }


             localizationService.localize(tokenKey).then(function(token) {
                localizationService.localize('merchelloSettings_stickListingTab', [ token ]).then(function(title) {
                    $scope.stickListTabTitle = title;
                    if ($scope.dialogData.settingsComponent !== undefined) {
                        var appender = angular.element( document.querySelector( '#settingsComponent' ) );
                        appender.append($scope.dialogData.settingsComponent);
                    }
                    $scope.loaded = true;
                });
             });
         }

         init();

}]);
