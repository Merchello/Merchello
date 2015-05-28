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


angular.module('merchello.models').constant('OfferProviderDisplay', OfferProviderDisplay);