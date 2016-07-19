angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionAddController',
    ['$scope', 'productAttributeDisplayBuilder',
    function($scope, productAttributeDisplayBuilder) {


        $scope.contentType = {};

        $scope.choiceName = '';

        $scope.defaultChoice = {};

        console.info($scope.dialogData);


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
                }

                var exists = _.find($scope.dialogData.choices, function(c) { return c.sku === choice.sku; });
                if (exists === undefined) {
                    $scope.dialogData.choices.push(choice);
                } else {

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
                    }
                }
            }
        }

        $scope.setSelectedChoice = function(choice) {
            _.each($scope.dialogData.choices, function (c) {
                if(c.sku === choice.sku) {
                    c.isDefaultChoice = true;
                } else {
                    c.isDefaultChoice = false;
                }
            });
        }

        $scope.sortableOptions = {
            start : function(e, ui) {
                ui.item.data('start', ui.item.index());
                console.info('start');
            },
            stop: function (e, ui) {
                var choice = ui.item.scope().choice;
                var start = ui.item.data('start'),
                    end =  ui.item.index();
                for(var i = 0; i < $scope.dialogData.choices.length; i++) {
                    $scope.dialogData.choices[i].sortOrder = i + 1;
                }
                console.info($scope.dialogData.choices);
            },
            disabled: false,
            cursor: "move"
        }

}]);
