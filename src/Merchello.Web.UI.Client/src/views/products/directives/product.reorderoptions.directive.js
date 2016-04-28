
    angular.module('merchello.directives').directive('productReorderOptions', [function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                isolateIsOpen: '=isOpen',
                product: '=',
                reload: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.reorderoptions.tpl.html',
            link: function ($scope, $element) {
                /**
                 * @ngdoc method
                 * @name close
                 * @function
                 *
                 * @description
                 * Set the isOpen scope property to false to close the dialog
                 */
                $scope.close = function ($event) {
                    $scope.isolateIsOpen = false;
                };

                // Settings for the sortable directive
                $scope.sortableOptions = {
                    update: function (e, ui) {
                        // Updating sortOrder of each productOption.
                        for (var i = 0; i < $scope.product.productOptions.length; i++) {
                            $scope.product.productOptions[i].sortOrder = i + 1;
                        }
                    },
                    axis: 'y',
                    cursor: "move"
                };

                $scope.sortableChoices = {
                    start: function (e, ui) {
                        $(e.target).data("ui-sortable").floating = true;    // fix for jQui horizontal sorting issue https://github.com/angular-ui/ui-sortable/issues/19
                    },
                    stop: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    update: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    cursor: "move"
                };
            }
        };
    }]);
