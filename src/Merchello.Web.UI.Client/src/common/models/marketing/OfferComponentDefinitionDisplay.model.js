/**
 * @ngdoc model
 * @name OfferComponentDefinitionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's OfferComponentDefinitionDisplay object
 */
var OfferComponentDefinitionDisplay = function() {
    var self = this;
    self.componentKey = '';
    self.name = '';
    self.description = '';
    self.typeFullName = '';
    self.typeGrouping = '';
    self.extendedData = {};
    self.componentType = '';
    self.dialogEditorView = {};
    self.restrictToType = '';
};

OfferComponentDefinitionDisplay.prototype = (function() {

    function clone() {
        return angular.extend(new OfferComponentDefinitionDisplay(), this);
    }

    return {
        clone: clone
    }
}());

angular.module('merchello.models').constant('OfferComponentDefinitionDisplay', OfferComponentDefinitionDisplay);