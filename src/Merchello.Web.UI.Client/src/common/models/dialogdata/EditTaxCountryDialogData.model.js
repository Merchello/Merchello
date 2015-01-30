    /**
     * @ngdoc model
     * @name EditTaxCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for editing a tax country
     *
     */
    var EditTaxCountryDialogData = function() {
        var self = this;
        self.country = {};
    };

   angular.module('merchello.models').constant('EditTaxCountryDialogData', EditTaxCountryDialogData);
