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
    self.offerSettingsKey = '';
    self.offerCode = '';
    self.componentKey = '';
    self.name = '';
    self.description = '';
    self.typeFullName = '';
    self.typeGrouping = '';
    self.displayConfigurationFormat = '';
    self.extendedData = {};
    self.componentType = '';
    self.dialogEditorView = {};
    self.restrictToType = '';
    self.requiresConfiguration = true;
    self.updated = false;
};

OfferComponentDefinitionDisplay.prototype = (function() {

    function clone() {
        return angular.extend(new OfferComponentDefinitionDisplay(), this);
    }

    function isConfigured() {

        if(!this.requiresConfiguration) {
            return true;
        }
        // hack catch for save call where there's a context switch on this to window
        // happens when saving the offer settings
        if (this.extendedData) {
            if (this.extendedData.items !== undefined && this.extendedData.items !== null) {
                return !this.extendedData.isEmpty();
            } else {
                return true;
            }
        } else {
            return true;
        }
    }

    return {
        clone: clone,
        isConfigured: isConfigured
    };
}());

angular.module('merchello.models').constant('OfferComponentDefinitionDisplay', OfferComponentDefinitionDisplay);