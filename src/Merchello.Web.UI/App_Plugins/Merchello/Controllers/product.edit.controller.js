/**
 * @ngdoc controller
 * @name Merchello.Editors.Product.EditController
 * @function
 * 
 * @description
 * The controller for the product editor
 */
function ProductEditController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    if ($routeParams.create) {
        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.product = {};
        $(".content-column-body").css('background-image', 'none');
    }
    else {

        //we are editing so get the product from the server
        var promise = merchelloProductService.getByKey($routeParams.id);

        promise.then(function (product) {

            $scope.product = product;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $(".content-column-body").css('background-image', 'none');

        }, function (reason) {

            alert('Failed: ' + reason.message);

        });
    }

    $scope.save = function () {

        $scope.$broadcast("Saving", { scope: $scope });

        //we are editing so get the product from the server
        var promise = merchelloProductService.save($scope.product);

        promise.then(function (product) {

            $scope.$broadcast("Product Saved", { scope: $scope });

        }, function (reason) {

            alert('Failed: ' + reason.message);

        });
    };

    $scope.addOption = function () {
        $scope.product.productOptions.push({ name: "", required: 0, sortOrder: $scope.product.productOptions.length + 1, choices: [{name: "one", sku: "one-sku", sortOrder: 1}] });
        createVariants();
    };

    createVariants = function () {
        
    };
}

angular.module("umbraco").$inject = ['tags-input'];

angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ProductEditController);