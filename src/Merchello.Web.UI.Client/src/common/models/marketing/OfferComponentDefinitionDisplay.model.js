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
    self.key = '';
    self.compoentKey = '';
    self.name = '';
    self.description = '';
    self.typeName = '';
    self.extendedData = {};
    self.editorView = {};
    self.restrictToType = '';
};

angular.module('merchello.models').constant('OfferComponentDefinitionDisplay', OfferComponentDefinitionDisplay);