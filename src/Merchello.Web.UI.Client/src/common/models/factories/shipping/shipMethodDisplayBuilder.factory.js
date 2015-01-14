    /**
     * @ngdoc service
     * @name merchello.models.shipMethodDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodDisplay models
     */
    angular.module('merchello.services')
        .factory('shipMethodDisplayBuilder',
            ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'shipProvinceDisplayBuilder', 'ShipMethodDisplay',
            function(genericModelBuilder, dialogEditorViewDisplayBuilder, shipProvinceDisplayBuilder, ShipMethodDisplay) {

                var Constructor = ShipMethodDisplay;

                return {
                    createDefault: function() {
                        var shipMethod = new Constructor();
                        shipMethod.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                        return shipMethod;
                    },
                    transform: function(jsonResult) {
                        var shipMethod = genericModelBuilder.transform(jsonResult, Constructor);
                        if (jsonResult.provinces) {
                            shipMethod.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                            shipMethod.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        }
                        return shipMethod;
                    }
                };

        }]);