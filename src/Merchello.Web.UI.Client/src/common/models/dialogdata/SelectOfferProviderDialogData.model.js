/**
 * @ngdoc model
 * @name SelectOfferProviderDialogData
 * @function
 *
 * @description
 * A dialogData model for use in the selecting an offer provider
 *
 */
var SelectOfferProviderDialogData = function() {
    var self = this;
    self.offerProviders = [];
    self.selectedProvider = {};
    self.warning = '';
};

angular.module('merchello.models').constant('SelectOfferProviderDialogData', SelectOfferProviderDialogData);