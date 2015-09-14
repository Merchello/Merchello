/**
 * @ngdoc model
 * @name DetachedContentTypeDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's DetachedContentTypeDisplay object
 */
var DetachedContentTypeDisplay = function () {
	var self = this;
	self.key = '';
	self.name = '';
	self.description = '';
	self.entityType = '';
    self.umbContentType = {};
};

angular.module('merchello.models').constant('DetachedContentTypeDisplay', DetachedContentTypeDisplay);