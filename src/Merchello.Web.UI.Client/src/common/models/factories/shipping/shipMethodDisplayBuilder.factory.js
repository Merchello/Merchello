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
                        if(jsonResult === undefined) {
                            return;
                        }
                        var shipMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        if (angular.isArray(jsonResult))
                        {
                            for(var i = 0; i < jsonResult.length; i++) {
                                shipMethods[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                                shipMethods[ i ].dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            }
                        } else {
                            shipMethods.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                            shipMethods.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        }
                        return shipMethods;
                    }
                };

        }]);