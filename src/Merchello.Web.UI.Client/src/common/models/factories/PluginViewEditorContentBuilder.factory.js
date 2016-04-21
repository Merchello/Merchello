angular.module('merchello.models').factory('pluginViewEditorContentBuilder',
    ['genericModelBuilder', 'PluginViewEditorContent',
    function(genericModelBuilder, PluginViewEditorContent) {

        var Constructor = PluginViewEditorContent;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var results = genericModelBuilder.transform(jsonResult, Constructor);
                if (angular.isArray(jsonResult)) {
                    angular.forEach(results, function(r) {
                        r.label = r.fileName;
                    });
                } else {
                    results.label = results.fileName;
                }

                return results;
            }
        };

}]);
