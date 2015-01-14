    /**
     * @ngdoc service
     * @name merchello.models.dialogEditorViewDisplayBuilder
     *
     * @description
     * A utility service that builds dialogEditorViewDisplay models
     */
    angular.module('merchello.models').factory('dialogEditorViewDisplayBuilder',
        ['genericModelBuilder', 'DialogEditorViewDisplay',
            function(genericModelBuilder, DialogEditorViewDisplay) {

                var Constructor = DialogEditorViewDisplay;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

