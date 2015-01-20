    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting ship countries to shipping configurations.
     */
    var DeleteShipCountryDialogData = function() {
        var self = this;
        self.country = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteShipCountryDialogData', DeleteShipCountryDialogData);
