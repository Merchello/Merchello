/**
 * @ngdoc controller
 * @name Merchello.Customer.Dialogs.CustomerCheckoutController
 * @function
 *
 * @description
 * The controller allowing to check a customer out from the back office
 */
angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerCheckoutController',
    ['$scope', '$filter',
    function($scope, $filter) {

        $scope.billingAddress = {};
        $scope.shippingAddress = {};

        // initializes the controller
        function init() {
            $scope.billingAddress = $scope.dialogData.customer.getDefaultBillingAddress();
            $scope.shippingAddress = $scope.dialogData.customer.getDefaultShippingAddress();

            // Get the email from the customer as it's not stored on the customer address type
            $scope.billingAddress.email = $scope.dialogData.customer.email;
            $scope.shippingAddress.email = $scope.dialogData.customer.email;
        }

        // formats a quote
        $scope.formatQuote = function(quote) {
            return quote.shipMethod.name + ' (' + $filter('currency')(quote.rate, $scope.dialogData.currencySymbol) + ')';
        }
        
        init();

    }]);