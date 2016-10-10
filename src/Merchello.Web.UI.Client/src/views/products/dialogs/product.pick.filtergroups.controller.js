angular.module('merchello').controller('Merchello.Product.Dialogs.PickSpecFilterCollectionsController',
    ['$scope',
    function($scope) {

        $scope.save = function() {
            // collections that have been modified will have attribute collections marked selected

            angular.forEach($scope.dialogData.available, function(collection) {
                setIntendedAssociations(collection);
            });

            $scope.submit($scope.dialogData.associate);
        }

        function setIntendedAssociations(collection) {
            var atts = _.filter(collection.filters, function(att) {
                if (att.selected) return att;
            });

            if (atts && atts.length > 0) {
                // we have to add the spec collection itself to be associated for certain system queries
                $scope.dialogData.associate.push(collection.key);
                angular.forEach(atts, function(a) {
                   $scope.dialogData.associate.push(a.key);
                });
            }
        }
}]);
