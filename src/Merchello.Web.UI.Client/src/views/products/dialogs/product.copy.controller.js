angular.module('merchello').controller('Merchello.Product.Dialogs.ProductCopyController',
    ['$scope', 'productResource',
    function($scope, productResource) {

        $scope.wasFormSubmitted = false;
        $scope.save = save;

        $scope.checking = false;
        $scope.isUnique = true;

        var currentSku = '';

        var input = angular.element( document.querySelector( '#copysku' ) );


        function init() {

            input.bind("keyup onfocusout", function (event) {
                var code = event.which;
                // alpha , numbers, ! and backspace

                if ( code === 45 ||
                    (code >47 && code <58) ||
                    (code >64 && code <91) ||
                    (code >96 && code <123) || code === 33 || code == 8) {
                    $scope.$apply(function () {
                        if ($scope.dialogData.sku !== '') {
                            checkUniqueSku($scope.dialogData.sku);
                        }
                    });
                } else {
                    event.preventDefault();
                }
            });
        }

        function save() {
            $scope.wasFormSubmitted = true;
            if ($scope.copyProductForm.name.$valid && $scope.copyProductForm.copysku.$valid && $scope.isUnique) {

                $scope.submit($scope.dialogData);
            }
        }

        function checkUniqueSku(sku) {

            $scope.checking = true;
            if (sku === undefined || sku === '') {
                $scope.checking = false;
                $scope.isUnique = true;
            } else {

                if (sku === currentSku) {
                    $scope.checking = false;
                    return true;
                }
                var checkPromise = productResource.getSkuExists(sku).then(function(exists) {
                    $scope.checking = false;
                    currentSku = sku;
                    $scope.isUnique = exists === 'false';
                    $scope.checking = false;
                });
            }
        }

        init();
}]);
