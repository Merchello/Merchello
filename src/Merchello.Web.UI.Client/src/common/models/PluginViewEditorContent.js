/**
 * @ngdoc model
 * @name PluginViewEditorContent
 * @function
 *
 * @description
 * Model for assisting in template (view) editting
 */
var PluginViewEditorContent = function() {
    var self = this;
    self.label = '';
    self.fileName = '';
    self.virtualPath = '';
    self.viewBody = '';
    self.viewType = '';
    self.modelTypeName = '';
};

PluginViewEditorContent.prototype = (function() {
    
    
    
}());

angular.module('merchello.models').constant('PluginViewEditorContent', PluginViewEditorContent);
