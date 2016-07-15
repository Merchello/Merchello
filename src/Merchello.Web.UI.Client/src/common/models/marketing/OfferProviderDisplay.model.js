/**
 * @ngdoc model
 * @name OfferProviderDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's OfferProviderDisplay object
 */
var OfferProviderDisplay = function() {
    var self = this;
    self.key = '';
    self.managesTypeName = '';
    self.backOfficeTree = {};
};

OfferProviderDisplay.prototype = (function() {

    function editorUrl(key) {
        return this.backOfficeTree.routePath.replace('{0}', key);
    }

    return {
        editorUrl : editorUrl
    };
}());

angular.module('merchello.models').constant('OfferProviderDisplay', OfferProviderDisplay);