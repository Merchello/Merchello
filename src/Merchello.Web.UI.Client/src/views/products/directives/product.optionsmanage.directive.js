    /**
     * @ngdoc controller
     * @name productOptionsManage
     * @function
     *
     * @description
     * The productOptionsManage directive
     */
    angular.module('merchello.directives').directive('productOptionsManage', function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                parentForm: '=',
                classes: '=',
                'update': '&onUpdate'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.optionsmanage.tpl.html',

            controller: function ($scope) {
                $scope.rebuildVariants = false;

                /**
                 * @ngdoc method
                 * @name addOption
                 * @function
                 *
                 * @description
                 * Called when the Add Option button is pressed.  Creates a new option ready to fill out.
                 */
                function addOption() {
                    $scope.rebuildVariants = true;
                    $scope.product.addBlankOption();
                }

                /**
                 * @ngdoc method
                 * @name removeOption
                 * @function
                 *
                 * @description
                 * Called when the Trash can icon button is pressed next to an option. Removes the option from the product.
                 */
                function removeOption (option) {
                    $scope.rebuildVariants = true;
                    $scope.product.removeOption(option);
                }

                /**
                 * @ngdoc method
                 * @name updateOptions
                 * @function
                 *
                 * @description
                 * Called when the update options button is pressed
                 */
                function updateOptions() {
                    $scope.update({ form: $scope.parentForm, rebuild: $scope.rebuildVariants });
                    $scope.rebuildVariants = false;
                }
            }
        };

    });
