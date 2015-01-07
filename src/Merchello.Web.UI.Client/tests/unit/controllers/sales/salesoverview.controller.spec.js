'use strict';

describe('salesoverview.controller', function() {
    var scope, $controllerConstructor, controller, httpBackend;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, $q, $httpBackend, assetsService, dialogService, localizationService, notificationsService,
                                localizationMocks, auditLogResource, auditLogResourceMock, invoiceResource, invoiceResourceMock,
                                paymentResource, paymentResourceMock, shipmentResource, settingsResource, settingResourceMock, salesHistoryDisplayBuilder,
                                paymentDisplayBuilder, invoiceDisplayBuilder) {

        httpBackend = $httpBackend;

        $controllerConstructor = $controller;
        scope = $rootScope.$new();

        invoiceResourceMock.register();
        auditLogResourceMock.register();
        paymentResourceMock.register();
        settingResourceMock.register();
        localizationMocks.register();

        controller = $controller('Merchello.Dashboards.SalesOverviewController',
            { $scope: scope, routeParams: { id: 'dd62d5a2-6a52-4de3-a740-193d2a25bbbb' }, $q: $q, assetsService: assetsService, notificationsService: notificationsService,
                auditLogResource: auditLogResource, invoiceResource: invoiceResource, paymentResource: paymentResource,
                shipmentResource: shipmentResource, settingsResource: settingsResource, salesHistoryDisplayBuilder: salesHistoryDisplayBuilder,
                paymentDisplayBuilder: paymentDisplayBuilder, invoiceDisplayBuilder: invoiceDisplayBuilder });

        //scope.$digest resolves the promise against the httpbackend
        scope.$digest();
        //httpbackend.flush() resolves all request against the httpbackend
        //to fake a async response, (which is what happens on a real setup)
        httpBackend.flush();

    }));

    it ('should load the invoice by default', function() {
        expect(scope.invoice).toBeDefined();
    });

});
