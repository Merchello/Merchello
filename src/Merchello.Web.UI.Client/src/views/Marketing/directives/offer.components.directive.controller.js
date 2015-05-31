/**
 * @ngdoc controller
 * @name Merchello.Directives.OfferComponentsDirectiveController
 * @function
 *
 * @description
 * The controller to handle offer component association and configuration
 */
angular.module('merchello').controller('Merchello.Directives.OfferComponentsDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'eventsService', 'dialogDataFactory', 'marketingResource', 'offerComponentDefinitionDisplayBuilder',
    function($scope, notificationsService, dialogService, eventsService, dialogDataFactory, marketingResource, offerComponentDefinitionDisplayBuilder) {

        $scope.componentsLoaded = false;
        $scope.availableComponents = [];
        $scope.assignedComponents = [];

        // exposed components
        $scope.assignComponent = assignComponent;
        $scope.removeComponentOpen = removeComponentOpen;
        $scope.configureComponentOpen = configureComponentOpen;

        var eventName = 'merchello.offercomponentcollection.changed';

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Initializes the controller
         */
        function init() {
            eventsService.on('merchello.offercomponentcollection.changed', onComponentCollectionChanged);

            $scope.$watch('preValuesLoaded', function(pvl) {
                if(pvl === true) {
                    loadComponents();
                }
            });
        }

        /**
         * @ngdoc method
         * @name loadComponents
         * @function
         *
         * @description
         * Loads the components for this offer
         */
        function loadComponents() {
            // either assigned constraints or rewards
            $scope.assignedComponents = _.filter($scope.offerSettings.componentDefinitions, function(osc) { return osc.componentType === $scope.componentType; });
            var typeGrouping = $scope.offerSettings.getComponentsTypeGrouping();

            $scope.availableComponents = _.filter($scope.components, function(c) {
                var ac = _.find($scope.assignedComponents, function(ac) { return ac.componentKey === c.componentKey; });
                if (ac === undefined && c.componentType === $scope.componentType && (typeGrouping === '' | typeGrouping === c.typeGrouping)) {
                    return c;
                }
            });

            $scope.componentsLoaded = true;
        }


        /**
         * @ngdoc method
         * @name assignComponent
         * @function
         *
         * @description
         * Adds a component from the offer
         */
        function assignComponent(component) {
            var assertComponent = _.find($scope.offerSettings.componentDefinitions, function(cd) { return cd.componentKey === component.componentKey; });
            if (assertComponent === undefined && $scope.offerSettings.ensureTypeGrouping(component.typeGrouping)) {
                $scope.offerSettings.componentDefinitions.push(component);
                eventsService.emit(eventName);
            }
        }

        /**
         * @ngdoc method
         * @name configureComponentOpen
         * @function
         *
         * @description
         * Opens the component configuration dialog
         */
        function configureComponentOpen(component) {
            var dialogData = dialogDataFactory.createConfigureOfferComponentDialogData();
            dialogData.component = component.clone();

            dialogService.open({
                template: component.dialogEditorView.editorView,
                show: true,
                callback: processConfigureComponent,
                dialogData: dialogData
            });

        }

        function processConfigureComponent(dialogData) {
            $scope.offerSettings.updateAssignedComponent(dialogData.component);
            console.info($scope.offerSettings.componentDefinitions);
        }

        /**
         * @ngdoc method
         * @name removeComponentOpen
         * @function
         *
         * @description
         * Opens the confirm dialog to a component from the offer
         */
        function removeComponentOpen(component) {
                var dialogData = {};
                dialogData.name = 'Component: ' + component.name;
                dialogData.componentKey = component.componentKey;
                if(!component.extendedData.isEmpty()) {
                    dialogData.warning = 'This will any delete any configurations for this component if saved.';
                }

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processRemoveComponent,
                    dialogData: dialogData
                });
        }

        /**
         * @ngdoc method
         * @name processRemoveComponent
         * @function
         *
         * @description
         * Removes a component from the offer
         */
        function processRemoveComponent(dialogData) {
            $scope.offerSettings.componentDefinitions = _.reject($scope.offerSettings.componentDefinitions, function(cd) { return cd.componentKey === dialogData.componentKey; })
            eventsService.emit(eventName);
        };

        function onComponentCollectionChanged() {
            loadComponents();
        }
        // Initialize the controller
        init();
    }]);