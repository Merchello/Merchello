/**
 * @ngdoc model
 * @name ProductVariantDetachedContentDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ProductVariantDetachedContentDisplay object
 */
var ProductVariantDetachedContentDisplay = function() {
    var self = this;
    self.key = '';
    self.detachedContentType = {};
    self.productVariantKey = '';
    self.cultureName = '';
    self.templateId = 0;
    self.slug = '';
    self.canBeRendered = true;
    self.detachedDataValues = {};
    self.uploadedFiles = [];
};


angular.module('merchello.models').constant('ProductVariantDetachedContentDisplay', ProductVariantDetachedContentDisplay);