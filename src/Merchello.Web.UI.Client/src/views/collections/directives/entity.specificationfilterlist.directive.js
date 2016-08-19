angular.module('merchello.directives').directive('entitySpecFilterList', [
    '$q', 'localizationService', 'eventsService', 'dialogService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    'entityCollectionProviderDisplayBuilder',
    function($q, localizationService, eventsService, dialogService, entityCollectionResource, entityCollectionDisplayBuilder,
             entityCollectionProviderDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                load: '&',
                doAdd: '&',
                doEdit: '&',
                entityType: '=',
                preValuesLoaded: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/entity.specificationfilterlist.tpl.html',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.noResults = '';
                scope.collections = [];
                scope.providers = [];

                /// PRIVATE
                var yes = '';
                var no = '';
                var attributes = '';


                scope.getColumnValue = function(col, spec) {
                    switch (col) {
                        case 'name':
                            return spec.name;
                        case 'attributes':
                            return spec.attributeCollections.length + ' ' + attributes;
                    };
                }

                scope.delete = function(collection) {
                    var dialogData = {
                        name: collection.name,
                        collection: collection
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                        show: true,
                        callback: processDeleteOption,
                        dialogData: dialogData
                    });
                }

                scope.add = function() {
                    var dialogData = {
                        providers: scope.providers,
                        entityType: scope.entityType,
                        selectedProvider: undefined,
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/select.specattributecollectionprovider.html',
                        show: true,
                        callback: openAddCollection,
                        dialogData: dialogData
                    });
                }


                function openAddCollection(provider) {
                    var collection = entityCollectionDisplayBuilder.createDefault()
                    collection.entityType = scope.entityType;
                    collection.entityTfKey = provider.entityTfKey;
                    collection.providerKey = provider.key;
                    var dialogData = {
                        attribute: collection
                    };

                    var template = provider.dialogEditorView.editorView !== '' ?
                                    provider.dialogEditorView.editorView : '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/specattributecollection.addedit.html';

                    dialogService.open({
                        template: template,
                        show: true,
                        callback: processAddAttribute,
                        dialogData: dialogData
                    });
                }

                function processDeleteOption(dialogData) {
                    entityCollectionResource.deleteEntityCollection(dialogData.collection.key).then(function(result) {
                        load();
                    });
                }

                function processAddAttribute(dialogData) {
                    console.info(dialogData);
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
                        localizationService.localize('merchelloSpecFilters_noSpecFilters'),
                        entityCollectionResource.getEntitySpecifiedFilterCollectionProviders(scope.entityType)
                    ]).then(function(data) {
                        yes = data[0];
                        no = data[1];
                        attributes = data[2];
                        scope.noResults = data[3];
                        scope.providers = entityCollectionProviderDisplayBuilder.transform(data[4]);
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
                    entityCollectionResource.getEntitySpecifiedFilterCollections(scope.entityType).then(function(results) {
                        scope.collections = entityCollectionDisplayBuilder.transform(results);
                        scope.loaded = true;
                    });

                }

                init();
            }
        }
    }]);
