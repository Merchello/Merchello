/**
 * @ngdoc controller
 * @name Merchello.Editors.Product.EditController
 * @function
 * 
 * @description
 * The controller for the product editor
 */
function ProductEditController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    $scope.productName = "Lego Star Wars: Ewok Village";

    if ($routeParams.create) {
        //we are creating so get an empty content item
        //dataTypeResource.getScaffold($routeParams.id, $routeParams.doctype)
         //   .then(function (data) {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.content = {};
                $(".content-column-body").css('background-image', 'none');
        //    });
    }
    else {


        //we are editing so get the content item from the server
        //dataTypeResource.getById($routeParams.id)
        //    .then(function (data) {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.content = {};
                $(".content-column-body").css('background-image', 'none');
        //        createPreValueProps($scope.content.preValues);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
         //       serverValidationManager.executeAndClearAllSubscriptions();
          //  });
    }

    $scope.save = function () {
        $scope.$broadcast("saving", { scope: $scope });


    };
}

angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ProductEditController);