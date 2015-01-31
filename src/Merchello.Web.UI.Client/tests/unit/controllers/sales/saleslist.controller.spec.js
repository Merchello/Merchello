'use strict';

describe('Merchello.Backoffice.SalesListController', function () {
    var scope, element, $controllerConstructor, controller, httpBackend;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, $httpBackend, $log, angularHelper, assetsService, notificationsService, invoiceResource, merchelloTabsFactory,
                                settingsResource, settingResourceMock, queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder, invoiceResourceMock, settingDisplayBuilder) {
        httpBackend = $httpBackend;

        $controllerConstructor = $controller;
        scope = $rootScope.$new();
        element = function() { };
        invoiceResourceMock.register();
        settingResourceMock.register();

        controller = $controller('Merchello.Backoffice.SalesListController',
            { $scope: scope, $element: element, $log: $log, angularHelper: angularHelper, settingsResource: settingsResource, assetsService: assetsService, notificationsService: notificationsService,
                merchelloTabsFactory: merchelloTabsFactory, invoiceResource: invoiceResource, queryDisplayBuilder: queryDisplayBuilder, queryResultDisplayBuilder: queryResultDisplayBuilder,
                invoiceDisplayBuilder: invoiceDisplayBuilder, settingDisplayBuilder: settingDisplayBuilder});

        //scope.$digest resolves the promise against the httpbackend
        scope.$digest();
        //httpbackend.flush() resolves all request against the httpbackend
        //to fake a async response, (which is what happens on a real setup)
        httpBackend.flush();

    }));

    it ('Init should load default invoices', function() {
        //// Assert
        expect (scope.currentPage = 1);
        expect (scope.itemCount).toBe(10);
        expect (scope.invoices[0].getPaymentStatus()).toBe('Paid');
    });

});

