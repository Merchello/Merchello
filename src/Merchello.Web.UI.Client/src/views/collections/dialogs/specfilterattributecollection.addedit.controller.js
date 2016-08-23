angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SpecFilterAttributeCollectionAddController',
    ['$scope', 'entityCollectionDisplayBuilder',
    function($scope, entityCollectionDisplayBuilder) {

        $scope.loaded = true;

        $scope.attributeName = '';
        $scope.wasFormSubmitted = false;

        $scope.save = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.collectionForm.name.$valid) {
                $scope.submit($scope.dialogData);
            }
        }

        $scope.addAttribute = function() {
            // first we need to clone the template so it can be reused (not modified)
            var attribute = angular.extend(entityCollectionDisplayBuilder.createDefault(), $scope.dialogData.attributeTemplate);
            attribute.name = $scope.attributeName;

            var exists = _.find($scope.dialogData.specCollection.attributeCollections, function(c) { return c.name === attribute.name; });
            if (!exists) {
                attribute.sortOrder = $scope.dialogData.specCollection.attributeCollections.length === 0 ?
                    0 :
                    $scope.dialogData.specCollection.attributeCollections.length;

                $scope.dialogData.specCollection.attributeCollections.push(attribute);
            }

            $scope.attributeName = '';

        }

        // removes a choice from the dialog data collection of choices
        $scope.remove = function(idx) {
            if ($scope.dialogData.specCollection.attributeCollections.length > idx) {
                var remover = $scope.dialogData.specCollection.attributeCollections[idx];
                var sort = remover.sortOrder;
                $scope.dialogData.specCollection.attributeCollections.splice(idx, 1);
                _.each($scope.dialogData.specCollection.attributeCollections, function(c) {
                    if (c.sortOrder > sort) {
                        c.sortOrder -= 1;
                    }
                });
            }
        };

        $scope.sortableAttributes = {
            start : function(e, ui) {
                ui.item.data('start', ui.item.index());
            },
            stop: function (e, ui) {
                var start = ui.item.data('start'),
                    end =  ui.item.index();
                for(var i = 0; i < $scope.dialogData.specCollection.attributeCollections.length; i++) {
                    $scope.dialogData.specCollection.attributeCollections[i].sortOrder = i + 1;
                }
            },
            disabled: false,
            cursor: "move"
        }
}]);
