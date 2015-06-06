/**
 * @ngdoc controller
 * @name Merchello.Directives.OfferComponentsDirectiveController
 * @function
 *
 * @description
 * The controller to handle offer component association and configuration
 */
angular.module('merchello').controller('Merchello.Directives.OfferComponentsDirectiveController',
    ['$scope', '$timeout', '$filter', 'notificationsService', 'dialogService', 'eventsService', 'dialogDataFactory', 'marketingResource', 'settingsResource', 'offerComponentDefinitionDisplayBuilder',
    function($scope, $timeout, $filter, notificationsService, dialogService, eventsService, dialogDataFactory, marketingResource, settingsResource, offerComponentDefinitionDisplayBuilder) {

        $scope.componentsLoaded = false;
        $scope.availableComponents = [];
        $scope.assignedComponents = [];
        $scope.currencySymbol = '';

        // exposed components methods
        $scope.assignComponent = assignComponent;
        $scope.removeComponentOpen = removeComponentOpen;
        $scope.configureComponentOpen = configureComponentOpen;
        $scope.isComponentConfigured = isComponentConfigured;
        $scope.applyDisplayConfigurationFormat = applyDisplayConfigurationFormat;

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
                   loadSettings();
                }
            });
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            var currencySymbolPromise = settingsResource.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;

                loadComponents();
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
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

            // there can only be one reward.
            if ($scope.componentType === 'Reward' && $scope.offerSettings.hasRewards()) {
                $scope.availableComponents = [];
                $scope.componentsLoaded = true;
                return;
            }

            $scope.availableComponents = _.filter($scope.components, function(c) {
                var ac = _.find($scope.assignedComponents, function(ac) { return ac.componentKey === c.componentKey; });
                if (ac === undefined && c.componentType === $scope.componentType && (typeGrouping === '' | typeGrouping === c.typeGrouping)) {
                    return c;
                }
            });

            $scope.componentsLoaded = true;
        }

        function applyDisplayConfigurationFormat(component) {
            if(component.displayConfigurationFormat !== undefined && component.displayConfigurationFormat !== '') {
                var value = eval(component.displayConfigurationFormat);
                if (value === undefined) {
                    return '';
                } else {
                    return value;
                }
            }
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
                component.offerSettingsKey = $scope.offerSettings.key;
                console.info(component);
                $scope.offerSettings.componentDefinitions.push(component);
                if ($scope.componentType === 'Reward') {
                    $scope.$parent.showApplyToEachMatching = true;
                }
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
            saveOffer();
            var component = _.find($scope.offerSettings.componentDefinitions, function(cd) { return cd.key === dialogData.component.key; } );
            component.updated = false;
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

        function isComponentConfigured(component) {
            if(!component.updated) {
                return component.isConfigured();
            }
        }

        function onComponentCollectionChanged() {
            loadComponents();
        }

        function saveOffer() {
            $timeout(function() {
                $scope.saveOfferSettings();
            }, 500);
        }
        // Initialize the controller
        init();
    }]);