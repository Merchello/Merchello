/**
 * @ngdoc model
 * @name ConfigureOfferComponentDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for configuring offer components.
 */
var ConfigureOfferComponentDialogData = function() {
    var self = this;
    self.component = {};
};

ConfigureOfferComponentDialogData.prototype = (function() {

    function setValue(key, value) {
        this.component.extendedData.setValue(key, value);
    }

    function getValue(key) {
        return this.component.extendedData.getValue(key);
    }

    return {
        setValue: setValue,
        getValue: getValue
    };
}());

angular.module('merchello.models').constant('ConfigureOfferComponentDialogData');