angular.module('merchello.directives').directive('entityFilterGroupList', [
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
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/entity.filtergrouplist.tpl.html',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.noResults = '';
                scope.collections = [];
                scope.providers = [];
                scope.hideDeletes = [];


                /// PRIVATE
                var yes = '';
                var no = '';
                var filterLabel = '';


                scope.getColumnValue = function(col, spec) {
                    switch (col) {
                        case 'name':
                            return spec.name;
                        case 'filters':
                            return spec.filters.length + ' ' + filterLabel;
                    };
                }

                scope.showDelete = function(spec) {
                    return !_.find(scope.hideDeletes, function(k) { return k === spec.providerKey; });
                }

                scope.delete = function(collection) {
                    var dialogData = {
                        name: collection.name,
                        collection: collection
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                        show: true,
                        callback: processDeleteCollection,
                        dialogData: dialogData
                    });
                }

                scope.add = function() {
                    var dialogData = {
                        providers: getValidProviders(),
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

                scope.edit = function(collection) {
                    // first we need to get the provider assigned to the filter attribute collections (child collections)
                    entityCollectionResource.getEntitySpecifiedFilterCollectionAttributeProvider(collection.key)
                        .then(function(result) {

                            var provider = entityCollectionProviderDisplayBuilder.transform(result);
                            var attributeTemplate = entityCollectionDisplayBuilder.createDefault();
                            attributeTemplate.providerKey = provider.key;
                            attributeTemplate.parentKey = collection.key;
                            attributeTemplate.entityType = scope.entityType;
                            attributeTemplate.entityTfKey = provider.entityTfKey;
                            attributeTemplate.isFilter = true;

                            var dialogData = {
                                provider: provider,
                                specCollection: collection.clone(),
                                attributeTemplate: attributeTemplate,
                                entityType: scope.entityType
                            };

                            var template = provider.dialogEditorView.editorView !== '' ?
                                provider.dialogEditorView.editorView : '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/specfilterattributecollection.addedit.html';


                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/specfilterattributecollection.addedit.html',
                                show: true,
                                callback: processEditCollection,
                                dialogData: dialogData
                            });
                    });

                }

                scope.openSort = function() {
                    var clones = [];
                    angular.forEach(scope.collections, function(c) {
                        clones.push(c.clone());
                    });

                    var dialogData = {
                        collections: clones
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sort.specfiltercollections.html',
                        show: true,
                        callback: processSortCollections,
                        dialogData: dialogData
                    });
                }

                function processSortCollections(collections) {
                    entityCollectionResource.updateSortOrders(collections).then(function() {
                       load();
                    });
                }

                function openAddCollection(provider) {
                    var collection = entityCollectionDisplayBuilder.createDefault()
                    collection.entityType = scope.entityType;
                    collection.entityTfKey = provider.entityTfKey;
                    collection.providerKey = provider.key;
                    collection.isFilter = true;
                    var dialogData = {
                        attribute: collection
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/specfiltercollection.add.html',
                        show: true,
                        callback: processAddCollection,
                        dialogData: dialogData
                    });
                }

                function processDeleteCollection(dialogData) {
                    var remover = dialogData.collection;
                    var updateSorts = _.filter(scope.collections, function (c) {
                       if (c.sortOrder > remover.sortOrder) {
                           c.sortOrder = c.sortOrder - 1;
                           return c;
                       }
                    });
                    scope.collections = _.reject(scope.collections, function (rem) { return rem.key === remover.key });

                    var promises = [entityCollectionResource.deleteEntityCollection(remover.key)];

                    if (updateSorts.length > 0) {
                        promises.push(entityCollectionResource.updateSortOrders(updateSorts));
                    }

                    $q.all(promises).then(function() {
                       load();
                    });
                }

                function processAddCollection(dialogData) {
                    var collection = dialogData.attribute;
                    collection.sortOrder = scope.collections.length;
                    console.info(collection);
                    scope.doAdd()(collection);
                }

                function processEditCollection(dialogData) {
                    scope.doEdit()(dialogData.specCollection);
                }

                function init() {

                    $q.all([
                        localizationService.localize('general_yes'),
                        localizationService.localize('general_no'),
                        localizationService.localize('merchelloTableCaptions_filters'),
                        localizationService.localize('merchelloSpecFilters_noSpecFilters'),
                        entityCollectionResource.getEntitySpecifiedFilterCollectionProviders(scope.entityType)
                    ]).then(function(data) {
                        yes = data[0];
                        no = data[1];
                        filterLabel = data[2];
                        scope.noResults = data[3];
                        scope.providers = entityCollectionProviderDisplayBuilder.transform(data[4]);
                        scope.hideDeletes =
                            _.pluck(
                                _.filter(scope.providers, function(p) {
                                    if (p.managesUniqueCollection) return p;
                                }),
                            'key');
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


                function getValidProviders() {
                    // providers that manage a unique collection may only ever be added once and should
                    // be automatically added by the bootstrapping
                    return _.filter(scope.providers, function(p) {

                        var usedByCollection = _.find(scope.collections, function (c) {
                           return c.providerKey === p.key;
                        });

                        // this is valid
                        if (!usedByCollection || (usedByCollection && !p.managesUniqueCollection)) {
                            return p;
                        }
                    });
                }




                function load() {
                    entityCollectionResource.getEntitySpecifiedFilterCollections(scope.entityType).then(function(results) {
                        scope.collections = entityCollectionDisplayBuilder.transform(results);
                        console.info(scope.collections);
                        scope.loaded = true;
                    });
                }

                init();
            }
        }
    }]);
