angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionAddController',
    ['$scope', 'productAttributeDisplayBuilder',
    function($scope, productAttributeDisplayBuilder) {

        $scope.contentType = {};
        $scope.choiceName = '';
        $scope.defaultChoice = {};
        $scope.selectedAttribute = {};
        $scope.wasFormSubmitted = false;


        // adds a choice to the dialog data collection of choicds
        $scope.addChoice = function() {

            if ($scope.choiceName !== '') {
                var choice = productAttributeDisplayBuilder.createDefault();
                choice.name = $scope.choiceName;
                choice.sku = $scope.choiceName.toLocaleLowerCase();
                choice.sortOrder = $scope.dialogData.choices.length + 1;

                if ($scope.dialogData.choices.length === 0) {
                    choice.isDefaultChoice = true;
                    $scope.defaultChoice = choice;
                    $scope.selectedAttribute = choice;
                }

                var exists = _.find($scope.dialogData.choices, function(c) { return c.sku === choice.sku; });
                if (exists === undefined) {
                    $scope.dialogData.choices.push(choice);
                }
            }

            $scope.choiceName = '';
        }

        // removes a choice from the dialog data collection of choices
        $scope.remove = function(idx) {
            if ($scope.dialogData.choices.length > idx) {
                var remover = $scope.dialogData.choices[idx];
                var sort = remover.sortOrder;
                var wasSelected = remover.isDefaultChoice;
                $scope.dialogData.choices.splice(idx, 1);
                _.each($scope.dialogData.choices, function(c) {
                   if (c.sortOrder > sort) {
                       c.sortOrder -= 1;
                   }
                });
                if (wasSelected) {
                    if($scope.dialogData.choices.length > 0) {
                        $scope.dialogData.choices[0].isDefaultChoice = true;
                        $scope.selectedAttribute = $scope.dialogData.choices[0];
                    }
                }
            }
        }

        // sets the default choice property
        $scope.setSelectedChoice = function(choice) {
            $scope.selectedAttribute.isDefaultChoice = false;
            choice.isDefaultChoice = true;
            $scope.selectedAttribute = choice;
        }


        // Saves an option
        $scope.save = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.productOptionForm.name.$valid) {
                // setup the option
                if ($scope.contentType.key) {
                    $scope.dialogData.detachedContentTypeKey = $scope.contentType.key;
                }

                $scope.submit($scope.dialogData);
            }
        }

        $scope.sortableOptions = {
            start : function(e, ui) {
                ui.item.data('start', ui.item.index());
            },
            stop: function (e, ui) {
                var choice = ui.item.scope().choice;
                var start = ui.item.data('start'),
                    end =  ui.item.index();
                for(var i = 0; i < $scope.dialogData.choices.length; i++) {
                    $scope.dialogData.choices[i].sortOrder = i + 1;
                }
            },
            disabled: false,
            cursor: "move"
        }

}]);
