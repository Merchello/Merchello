angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.FilterGroupAddEditFilterController',
    ['$scope', 'entityCollectionDisplayBuilder',
    function($scope, entityCollectionDisplayBuilder) {

        $scope.loaded = true;

        $scope.filterName = '';
        $scope.wasFormSubmitted = false;

        $scope.save = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.collectionForm.name.$valid) {
                $scope.submit($scope.dialogData);
            }
        }

        $scope.addFilter = function() {
            // first we need to clone the template so it can be reused (not modified)
            var filter = angular.extend(entityCollectionDisplayBuilder.createDefault(), $scope.dialogData.filterTemplate);
            filter.name = $scope.filterName;

            var exists = _.find($scope.dialogData.filterGroup.filters, function(c) { return c.name === filter.name; });
            if (!exists) {
                filter.sortOrder = $scope.dialogData.filterGroup.filters.length === 0 ?
                    0 :
                    $scope.dialogData.filterGroup.filters.length;

                $scope.dialogData.filterGroup.filters.push(filter);
            }

            $scope.filterName = '';

        }

        // removes a choice from the dialog data collection of choices
        $scope.remove = function(idx) {
            if ($scope.dialogData.filterGroup.filters.length > idx) {
                var remover = $scope.dialogData.filterGroup.filters[idx];
                var sort = remover.sortOrder;
                $scope.dialogData.filterGroup.filters.splice(idx, 1);
                _.each($scope.dialogData.filterGroup.filters, function(c) {
                    if (c.sortOrder > sort) {
                        c.sortOrder -= 1;
                    }
                });
            }
        };

        $scope.sortableFilters = {
            start : function(e, ui) {
                ui.item.data('start', ui.item.index());
            },
            stop: function (e, ui) {
                var start = ui.item.data('start'),
                    end =  ui.item.index();
                for(var i = 0; i < $scope.dialogData.filterGroup.filters.length; i++) {
                    $scope.dialogData.filterGroup.filters[i].sortOrder = i + 1;
                }
            },
            disabled: false,
            cursor: "move"
        }
}]);
