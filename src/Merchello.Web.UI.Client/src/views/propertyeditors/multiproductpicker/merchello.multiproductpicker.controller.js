angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerController',
    ['$scope', 'dialogService', 'productResource', 'productDisplayBuilder',
    function($scope, dialogService, productResource, productDisplayBuilder) {

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


            if ($scope.model.value !== undefined && $scope.model.value !== '' && $scope.model.value.length > 0) {
                $scope.keys = $scope.model.value;
            }

            if ($scope.keys.length > 0) {
                productResource.getByKeys($scope.keys).then(function(data) {
                    $scope.products = productDisplayBuilder.transform(data);
                });
            }
        }

        function remove(product) {
            $scope.products = _.reject($scope.products, function (p) { return p.key === product.key });
            setModelValue();
        }

        function openPickerDialog() {
            var dialogData = {};
            var dataProducts = [];
            angular.copy($scope.products, dataProducts);
            dialogData.products = dataProducts;
            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/multiproductpicker/merchello.multiproductpicker.dialog.html',
                show: true,
                callback: selectProductFromDialog,
                dialogData: dialogData
            });
        }

        function selectProductFromDialog(dialogData) {
            $scope.products = dialogData.products;
            setModelValue();
        }

        function setModelValue() {
            $scope.model.value = _.pluck($scope.products, 'key');
        }

        init();

    }]);
