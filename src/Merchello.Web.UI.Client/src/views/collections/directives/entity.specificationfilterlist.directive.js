angular.module('merchello.directives').directive('entitySpecFilterList', [
    '$q', 'localizationService', 'eventsService', 'dialogService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    function($q, localizationService, eventsService, dialogService, entityCollectionResource, entityCollectionDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                load: '&',
                doAdd: '&',
                doEdit: '&',
                doDelete: '&',
                entityType: '=',
                preValuesLoaded: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/entity.specificationfilterlist.tpl.html',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.noResults = '';
                scope.collections = [];
                /// PRIVATE
                var yes = '';
                var no = '';
                var attributes = '';


                scope.getColumnValue = function(col, spec) {
                    switch (col) {
                        case 'name':
                            return spec.name;
                        case 'attributes':
                            return spec.attributeCollections.length + ' ' + values;
                    };
                }

                scope.add = function() {
                    var dialogData = {
                        attribute: entityCollectionDisplayBuilder.createDefault(),
                        entityType: scope.entityType
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/select.specattributecollectionprovider.html',
                        show: true,
                        callback: processAddAttribute,
                        dialogData: dialogData
                    });
                }



                function processDeleteOption(dialogData) {
                    scope.doDelete()(dialogData.option);
                }

                function processAddAttribute(dialogData) {
                    scope.doAdd()(dialogData.attribute);
                }

                function processEditOption(dialogData) {
                    scope.doEdit()(dialogData.option);
                }

                function init() {

                    $q.all([
                        localizationService.localize('general_yes'),
                        localizationService.localize('general_no'),
                        localizationService.localize('merchelloTableCaptions_filterSpecAttributes'),
                        localizationService.localize('merchelloSpecFilters_noSpecFilters')
                    ]).then(function(data) {
                        yes = data[0];
                        no = data[1];
                        values = data[2];
                        scope.noResults = data[3];

                    });

                    scope.$watch('preValuesLoaded', function(nv, ov) {
                        if (nv === true) {
                            scope.isReady = true;
                        } else {
                            scope.isReady = false;
                        }

                        if (scope.isReady) {
                            load();
                        }
                    });
                }

                function load() {
                    entityCollectionResource.getEntitySpecificationCollections(scope.entityType).then(function(results) {
                        scope.collections = entityCollectionDisplayBuilder.transform(results);
                        scope.loaded = true;
                    });

                }

                init();
            }
        }
    }]);
