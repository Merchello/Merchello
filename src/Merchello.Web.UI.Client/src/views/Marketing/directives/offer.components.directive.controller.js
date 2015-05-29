/**
 * @ngdoc controller
 * @name Merchello.Directives.OfferComponentsDirectiveController
 * @function
 *
 * @description
 * The controller to handle offer component association and configuration
 */
angular.module('merchello').controller('Merchello.Directives.OfferComponentsDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'marketingResource', 'offerComponentDefinitionDisplayBuilder',
    function($scope, notificationsService, dialogService, marketingResource, offerComponentDefinitionDisplayBuilder) {

        $scope.componentsLoaded = false;
        $scope.avalailableComponents = [];
        $scope.assignedComponents = [];

        // exposed components
        $scope.assignComponent = assignComponent;
        $scope.removeComponentOpen = removeComponentOpen;

        var allComponents = [];

        function init() {
            $scope.$watch('offerSettings', function(offer) {
                if (offer.offerProviderKey !== undefined && offer.offerProviderKey !== '') {
                    console.info($scope.componentType);
                  loadComponents();
                }
            });
        }

        function loadComponents() {

            $scope.assignedComponents = _.filter($scope.offerSettings.componentDefinitions, function(osc) { return osc.componentType === $scope.componentType; });

            var componentPromise = marketingResource.getAvailableOfferComponents($scope.offerSettings.offerProviderKey);
            componentPromise.then(function(components) {
                console.info(components);
                allComponents = offerComponentDefinitionDisplayBuilder.transform(components);
                console.info(allComponents);
                $scope.avalailableComponents = _.filter(allComponents, function(c) {
                    var ac = _.find($scope.assignedComponents, function(ac) { return ac.key === c.key; });
                    if (ac === undefined && c.componentType === $scope.componentType) {
                        return c;
                    }
                });
                $scope.componentsLoaded = true;
            }, function(reason) {
                notificationsService.error("Failted to load offer offer components", reason.message);
            });
        }

        function assignComponent(component) {
            var assert = _.find($scope.offerSettings.componentDefinitions, function(cd) { return cd.key === component.key; });
            if (assert === undefined) {
                $scope.offerSettings.componentDefinitions.push(component);
                loadComponents();
            }
        }

        function removeComponentOpen(component) {
                var dialogData = {};
                dialogData.name = 'Component: ' + component.name;
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

        function processRemoveComponent(dialogData) {
            
        };

        // Initialize the controller
        init();
    }]);