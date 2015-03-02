    /**
     * @ngdoc model
     * @name EditShippingGatewayMethodDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for shipment gateway method data to the dialogService
     *
     */
    var EditShippingGatewayMethodDialogData = function() {
        var self = this;
        self.shippingGatewayMethod = {};
        self.currencySymbol = '';
    };

    angular.module('merchello.models').constant('EditShippingGatewayMethodDialogData', EditShippingGatewayMethodDialogData);