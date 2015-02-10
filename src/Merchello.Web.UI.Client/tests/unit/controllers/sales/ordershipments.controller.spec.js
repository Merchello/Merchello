'use strict';

describe('Merchello.Backoffice.OrderShipmentsController', function () {
    var scope, element, $controllerConstructor, controller, httpBackend;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, $httpBackend, $log, dialogService, notificationsService, invoiceResource, dialogDataFactory,
                                merchelloTabsFactory, invoiceDisplayBuilder, invoiceResourceMock, settingsResource, settingResourceMock,
                                shipmentResource, shipmentResourceMock, shipmentDisplayBuilder){

        httpBackend = $httpBackend;
        $controllerConstructor = $controller;
        scope = $rootScope.$new();

        invoiceResourceMock.register();
        settingResourceMock.register();
        shipmentResourceMock.register();

        controller = $controller('Merchello.Backoffice.OrderShipmentsController',
            { $scope: scope, $routeParams: { id: 'dd62d5a2-6a52-4de3-a740-193d2a25bbbb' }, $log: $log, notificationsService: notificationsService,
                dialogService: dialogService, invoiceResource: invoiceResource, dialogDataFactory: dialogDataFactory, merchelloTabsFactory: merchelloTabsFactory,
                settingsResource: settingsResource, shipmentResource: shipmentResource,
                invoiceDisplayBuilder: invoiceDisplayBuilder, shipmentDisplayBuilder: shipmentDisplayBuilder});

        //scope.$digest resolves the promise against the httpbackend
        scope.$digest();
        //httpbackend.flush() resolves all request against the httpbackend
        //to fake a async response, (which is what happens on a real setup)
        httpBackend.flush();
    }));

    it('should load an invoice on init', function() {
        expect(scope.invoice).toBeDefined();
    })

    xit('should load a shipments array by default', function() {
        expect(scope.shipments.length).toBeGreaterThan(0);
        //console.info(scope.shipments)
    });
});
