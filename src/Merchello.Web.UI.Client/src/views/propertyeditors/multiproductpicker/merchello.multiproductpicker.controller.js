angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerController',
    ['$scope', '$q', 'dialogService', 'settingsResource', 'productResource', 'productDisplayBuilder',
    function($scope, $q, dialogService, settingsResource, productResource, productDisplayBuilder) {

        $scope.preValuesLoaded = false;
        $scope.keys = [];
        $scope.products = [];
        $scope.settings = {};
        $scope.remove = remove;
        $scope.openPickerDialog = openPickerDialog;

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
        $scope.currentPage = 0;
        $scope.maxPages = 0;

        function init() {
            if ($scope.model.value !== '') {
                $scope.keys = JSON.parse($scope.model.value);
            }

            if ($scope.keys.length > 0) {
                $q.all([
                    settingsResource.getAllSettings(),
                    productResource.getByKeys($scope.keys)
                ]).then(function(data) {
                    $scope.settings = data[0].settings;
                    console.info($scope.settings);
                });
            }
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            // this is needed for the date format
            var settingsPromise = settingsResource.getAllSettings();

        }

        function loadProducts() {

        }


        function remove(product) {

        }

        function openPickerDialog() {
            var dialogData = {};
            dialogData.products = $scope.products;
            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/multiproductpicker/merchello.multiproductpicker.dialog.html',
                show: true,
                callback: selectProductFromDialog,
                dialogData: dialogData
            });
        }

        function selectProductFromDialog(dialogData) {

        }

        init();

    }]);
