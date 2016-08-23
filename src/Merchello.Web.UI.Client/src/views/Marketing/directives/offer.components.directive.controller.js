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
        $scope.partition = [];
        $scope.currencySymbol = '';
        $scope.sortComponent = {};

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
            eventsService.on(eventName, onComponentCollectionChanged);

            // ensure that the parent scope promises have been resolved
            $scope.$watch('preValuesLoaded', function(pvl) {
                if(pvl === true) {
                   loadSettings();
                }
            });

            // if these are constraints, enable the sort
            if ($scope.componentType === 'Constraint') {
                $scope.sortableOptions.disabled = false;
            }
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
            if($scope.offerSettings.assignComponent(component))
            {
                if ($scope.componentType === 'Reward') {
                    $scope.$parent.hasReward = true;
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
            eventsService.unsubscribe(loadComponents);
        }

        function saveOffer() {
            $timeout(function() {
                $scope.saveOfferSettings();
            }, 500);
        }

        // Sortable available offers
        /// -------------------------------------------------------------------

        $scope.sortableOptions = {
            start : function(e, ui) {
               ui.item.data('start', ui.item.index());
            },
           stop: function (e, ui) {
               var component = ui.item.scope().component;
               var start = ui.item.data('start'),
                   end =  ui.item.index();
               // reorder the offerSettings.componentDefinitions
               if ($scope.offerSettings.hasRewards()) {
                   // the reward is always in position 0
                   start++;
                   end++;
               }
               $scope.offerSettings.reorderComponent(start, end);
            },
            disabled: true,
            cursor: "move"
        }

        // Initialize the controller
        init();
    }]);