angular.module('merchello').controller('Merchello.Sales.Dialogs.ManageAdjustmentsController',
    ['$scope', '$filter', 'invoiceLineItemDisplayBuilder',
    function($scope, $filter, invoiceLineItemDisplayBuilder) {

        $scope.deleteAdjustment = deleteAdjustment;
        $scope.addAdjustment = addAdjustment;

        $scope.preValuesLoaded = true;
        $scope.save = save;
        $scope.operator = '-';
        $scope.invoiceNumber = '';
        $scope.adjustments = [];
        $scope.amount = 0.0;
        $scope.sku = 'adj';
        $scope.lineItemType = '';
        $scope.isTaxable = false;
        //$scope.lineItemTypes = [];
        $scope.amount = 0.0;
        $scope.name = '';

        function init() {
            // Setup the Adjustment types
            //$scope.lineItemTypes = ["Adjustment", "Shipping", "Tax"];

            // Set the default type
            $scope.lineItemType = "Adjustment";

            $scope.invoiceNumber = $scope.dialogData.invoice.prefixedInvoiceNumber();
            var adjustments = $scope.dialogData.invoice.getAdjustmentLineItems();
            if (adjustments !== undefined && adjustments !== null) {
                $scope.adjustments = adjustments;
            }
        }
        
        function deleteAdjustment(item) {
            if (item.isNew !== undefined) {
                $scope.adjustments = _.reject($scope.adjustments, function(adj) {
                   return adj.isNew === true && adj.name === item.name;
                });
            } else {
                $scope.adjustments = _.reject($scope.adjustments, function(adj) {
                   return adj.key === item.key;
                });
            }
        }

        function addAdjustment() {
            if ($scope.name !== '') {
                var lineItem = invoiceLineItemDisplayBuilder.createDefault();
                lineItem.quantity = 1;
                lineItem.isTaxable = $scope.isTaxable;
                lineItem.name = $scope.name;
                lineItem.containerKey = $scope.dialogData.invoice.key;
                lineItem.lineItemType = $scope.lineItemType;
                var amount = Math.abs($scope.amount);
                lineItem.price = $scope.operator === '+' ? amount : -1 * amount;
                lineItem.sku = $scope.sku;
                lineItem.isNew = true;
                $scope.adjustments.push(lineItem);

                // Reset
                $scope.name = '';
                $scope.amount = 0;
                $scope.operator = '-';
                $scope.sku = 'adj';
                //$scope.lineItemType = $scope.lineItemType;
            }
        }

        function save() {
            
            var items = [];
            _.each($scope.adjustments, function(adj) {
                items.push({ key: adj.key, name: adj.name, price: adj.price, sku: adj.sku, lineItemType: adj.lineItemType, isTaxable: adj.isTaxable });
            });

            var invoiceAdjustmentDisplay = {
                invoiceKey: $scope.dialogData.invoice.key,
                items: items
            };
            $scope.submit(invoiceAdjustmentDisplay);
        }

        init();

    }]);
