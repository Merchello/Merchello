angular.module('merchello.directives').directive('entitySpecifiedFilters',
    function($q, dialogService, entityCollectionResource, entityCollectionDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                preValuesLoaded: '=',
                entity: '=',
                entityType: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.specifiedfilters.tpl.html',
            link: function(scope, elm, attr) {

                scope.ready = false;
                scope.available = [];
                scope.associated = [];
                scope.currentFilters = [];

                scope.addOrRemove = function(collection, att) {
                    if (att.selected) {
                        entityCollectionResource.addEntityToCollection(scope.entity.key, att.key);
                    } else {
                        var others = _.filter(collection.attributeCollections, function(ac) {
                           if (ac.selected && ac.key !== att.key) return ac;
                        });
                        var promises = [];
                        promises.push(entityCollectionResource.removeEntityFromCollection(scope.entity.key, att.key));
                        if (others.length === 0) {
                            promises.push(entityCollectionResource.removeEntityFromCollection(scope.entity.key, collection.key));
                        }
                        $q.all(promises).then(function(result) {
                           load();
                        });
                    }
                }

                scope.associateFilters = function() {

                        var dialogData = {
                            available: getAvailableClone(),
                            associate: []
                        };

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/product.pick.specfiltercollections.html',
                            show: true,
                            callback: addAssociation,
                            dialogData: dialogData
                        });
                }

                function addAssociation(associations) {
                    if (!angular.isArray(associations)) return;
                    if (associations.length > 0) {
                        scope.ready = false;
                        entityCollectionResource.associateEntityWithFilterCollections(scope.entity.key, associations).then(function(result) {
                            load();
                        });
                    }
                }

                function init() {
                    scope.$watch('preValuesLoaded', function(nv, ov) {
                       if (nv === true) {
                           load();
                       }
                    });
                }

                function load() {
                    $q.all([
                        entityCollectionResource.getSpecifiedFilterCollectionsContainingProduct(scope.entityType, scope.entity.key),
                        entityCollectionResource.getSpecifiedFilterCollectionsNotContainingProduct(scope.entityType, scope.entity.key),
                        entityCollectionResource.getEntityCollectionsByEntity(scope.entity, scope.entityType, true)

                    ])
                    .then(function(data) {
                        scope.associated = entityCollectionDisplayBuilder.transform(data[0]);
                        scope.available = entityCollectionDisplayBuilder.transform(data[1]);
                        scope.currentFilters = entityCollectionDisplayBuilder.transform(data[2]);

                        // keep the directive hidden if there is nothing to do
                        if (scope.associated.length > 0 || scope.available.length > 0) {

                            // add the selected property to each of the attribute collections

                            // associated filters
                            angular.forEach(scope.associated, function(asf) {
                                // the root collection that represents the filter group
                                asf.selected = true;

                                angular.forEach(asf.attributeCollections, function(asfac) {
                                    var fnd = _.find(scope.currentFilters, function(current) { return current.key === asfac.key; });
                                    asfac.selected = fnd !== undefined;
                                });
                            });

                            // available filters
                            angular.forEach(scope.available, function(avf) {
                                avf.selected = false;
                                angular.forEach(avf.attributeCollections, function(avfac) {
                                   avfac.selected = false;
                                });
                            });

                            scope.ready = true;
                        }
                    });

                }

                function getAvailableClone() {
                    var clone = [];
                    angular.forEach(scope.available, function(av) {

                        var avcopy = cloneCollection(av);
                        avcopy.attributeCollections = [];
                        angular.forEach(av.attributeCollections, function(avac) {
                            avcopy.attributeCollections.push(cloneCollection(avac));
                        });
                        clone.push(avcopy);
                    });

                    return clone;

                    function cloneCollection(collection) {
                        var tmp = entityCollectionDisplayBuilder.createDefault();
                        var copy = angular.extend(tmp, collection);
                        return copy;
                    }
                }


                //
                init();
            }
        }
        });
