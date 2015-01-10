'use strict';

xdescribe('salesoverview.controller', function() {
    var scope, controller, httpBackend;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, $httpBackend, $timeout, assetsService, dialogService, localizationService, notificationsService,
                                settingsResource, shipmentResourceMock,
                                localizationMocks, auditLogResource, auditLogResourceMock, invoiceResource, invoiceResourceMock,
                                paymentResource, paymentResourceMock, shipmentResource, settingResourceMock, dialogDataFactory, salesHistoryDisplayBuilder,
                                invoiceDisplayBuilder, paymentDisplayBuilder, shipMethodsQueryDisplay) {

        httpBackend = $httpBackend;

        scope = $rootScope.$new();

        shipmentResourceMock.register();
        invoiceResourceMock.register();
        auditLogResourceMock.register();
        paymentResourceMock.register();
        settingResourceMock.register();
        localizationMocks.register();

        controller = $controller('Merchello.Dashboards.SalesOverviewController',
            { $scope: scope, $routeParams: { id: 'dd62d5a2-6a52-4de3-a740-193d2a25bbbb' }, $timeout: $timeout,
                assetsService: assetsService, notificationsService: notificationsService, dialogService: dialogService, localizationService: localizationService,
                auditLogResource: auditLogResource, invoiceResource: invoiceResource, settingsResource: settingsResource,
                paymentResource: paymentResource, shipmentResource: shipmentResource, dialogDataFactory: dialogDataFactory, paymentDisplayBuilder: paymentDisplayBuilder,
                salesHistoryDisplayBuilder: salesHistoryDisplayBuilder, invoiceDisplayBuilder: invoiceDisplayBuilder, shipMethodsQueryDisplay: shipMethodsQueryDisplay
                 });

        //scope.$digest resolves the promise against the httpbackend
        scope.$digest();

        //httpbackend.flush() resolves all request against the httpbackend
        //to fake a async response, (which is what happens on a real setup)
        httpBackend.flush();
    }));

    afterEach(inject(function($httpBackend) {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    }));

    it ('should load the invoice by default', function() {

        //// Assert
        expect(scope.invoice).toBeDefined();
        console.info(scope.invoice);
        //expect(scope.invoice.getPaymentStatus()).toBe('Paid');
        //expect(scope.invoice.hasOrder()).toBeFalsy();
        //expect(scope.invoice.isPaid()).toBeTruthy();
    });

    it('should set the currencySymbol', function() {
        //// Assert
        expect(scope.currencySymbol).toBe('$');
    });

    it('billing address should be defined on the scope', function() {
        //// Assert
        expect(scope.billingAddress).toBeDefined();
        console.info(scope.payments);
    });

});
