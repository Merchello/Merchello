angular.module('merchello').controller('Merchello.Common.Dialogs.ListViewSettingsController',
    ['$scope', '$compile', '$element',
     function($scope, $compile, $element) {

     if ($scope.dialogData.settingsComponent !== undefined) {
         var appender = angular.element( document.querySelector( '#settingsComponent' ) );
         appender.append($scope.dialogData.settingsComponent);
     }

}]);
