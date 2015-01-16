    /**
     * @ngdoc model
     * @name DeleteShipCountryShipMethodDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting ship countries ship methods.
     */
    var DeleteShipCountryShipMethodDialogData = function() {
        var self = this;
        self.shipMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteShipCountryShipMethodDialogData', DeleteShipCountryShipMethodDialogData);