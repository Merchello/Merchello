angular.module('merchello.directives').directive('entitySpecFilterList', [
    '$q', 'localizationService', 'eventsService', 'dialogService', 'entityCollectionResource',
    function($q, localizationService, eventsService, dialogService, entityCollectionResource) {
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

                /// PRIVATE
                var yes = '';
                var no = '';
                var values = '';

                function processDeleteOption(dialogData) {
                    scope.doDelete()(dialogData.option);
                }

                function processAddOption(dialogData) {
                    scope.doAdd()(dialogData.option);
                }

                function processEditOption(dialogData) {
                    scope.doEdit()(dialogData.option);
                }

                function init() {

                    $q.all([
                        localizationService.localize('general_yes'),
                        localizationService.localize('general_no'),
                        localizationService.localize('merchelloTableCaptions_optionValues'),
                        entityCollectionResource.getEntitySpecificationCollections(scope.entityType)
                        //localizationService.localize('merchelloProductOptions_' + noResultsKey)
                    ]).then(function(data) {
                        yes = data[0];
                        no = data[1];
                        values = data[2];
                        console.info(data[3]);
                        //scope.noResults = data[3];
                        scope.loaded = true;
                    });

                    scope.$watch('preValuesLoaded', function(nv, ov) {
                        if (nv === true) {
                            scope.isReady = true;
                        } else {
                            scope.isReady = false;
                        }

                        if (scope.isReady) {
                            scope.search();
                        }
                    });
                }

                scope.search = function() {


                }

                init();
            }
        }
    }]);
