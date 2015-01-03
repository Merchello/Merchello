'use strict';

describe('Merchello.Dashboards.Sales.ListController', function () {
    var scope, $controllerConstructor, httpBackend;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, $httpBackend, invoiceResourceMock) {
        httpBackend = $httpBackend;

        $controllerConstructor = $controller;
        scope = $rootScope.$new();
        invoiceResourceMock.register();

    }));


    it ('Init should load default invoices', inject(function(angularHelper, assetsService, notificationsService, invoiceResource,
                                                             queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder, invoiceMocks) {

        //// Arrange
        var jsonResult = invoiceMocks.invoicesArray();

        var ctl = $controllerConstructor('Merchello.Dashboards.Sales.ListController',
            { $scope: scope, angularHelper: angularHelper, assetsService: assetsService, notificationsService: notificationsService,
                invoiceResource: invoiceResource, queryDisplayBuilder: queryDisplayBuilder, queryResultDisplayBuilder: queryResultDisplayBuilder,
                invoiceDisplayBuilder: invoiceDisplayBuilder});

        //scope.$digest resolves the promise against the httpbackend
        scope.$digest();
        //httpbackend.flush() resolves all request against the httpbackend
        //to fake a async response, (which is what happens on a real setup)
        httpBackend.flush();

        //// Assert
        //console.info(scope.invoices);
        console.info(scope.invoices);
        console.info(scope.invoices[0].getPaymentStatus());
        expect (scope.currentPage = 1);

    }));


});

