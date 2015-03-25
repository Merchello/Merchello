    /**
     * @ngdoc controller
     * @name Merchello.Marketing.Dialogs.AddEditCampaignController
     * @function
     *
     * @description
     * The controller for the adding / editing marketing campaigns
     */
    angular.module('merchello').controller('Merchello.Marketing.Dialogs.AddEditCampaignController',
        ['$scope',
        function($scope) {

            $scope.manuallyDefineAlias = false;

            // exposed methods
            $scope.save = save;

            function init() {
                console.info($scope.dialogData);
                if ($scope.dialogData.isEdit()) {
                    $scope.manuallyDefineAlias = true;
                }
            }

            function save() {
                $scope.editCampaignForm.name.$valid
                if ($scope.dialogData.campaign.alias === '') {
                    $scope.dialogData.generateAlias();
                }
                console.info($scope.dialogData);
                $scope.submit($scope.dialogData);
            }

            // initialize the controller
            init();
    }])