    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

        $scope.init = function() {
            //console.info($scope.dialogData);
            $scope.setVariables();
        };

        $scope.setVariables = function() {
            $scope.address = $scope.dialogData.address;
        };

        $scope.save = function() {
            $scope.submit();
        };

        $scope.init();
    });

