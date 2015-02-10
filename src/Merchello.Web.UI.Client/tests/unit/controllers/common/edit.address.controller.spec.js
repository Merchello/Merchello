'use strict';

describe("Merchello.Common.Dialogs.EditAddressController", function () {

    var scope, $controllerConstructor;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, dialogDataMocks) {

        $controllerConstructor = $controller;
        var dialogData = dialogDataMocks.getAddressDialogData();

        scope = $rootScope.$new();
        scope.dialogData = dialogData;
    }));

    it ('should set $scope.address to the dialogData address', function() {
        //// Arrange
        var ctl = $controllerConstructor("Merchello.Common.Dialogs.EditAddressController", { $scope: scope });

        //// Assert
        expect (scope.address.name).toBe('Disney World');
    });

    it ('should update the dialogData.address to the scope address on save', function() {

        //// Arrange
        var ctl = $controllerConstructor("Merchello.Common.Dialogs.EditAddressController", { $scope: scope });
        var name = 'Mindfly, Inc.'
        var address1 = '114 W. Magnolia St.';
        var address2 = 'Suite 300';
        var locality = 'Bellingham';
        var region = 'WA';
        var postalCode = '98225';
        var countryCode = 'US';
        var organization = 'Mindfly, Inc.';
        var addressType = 'shipping';
        var phone = '(555) 555-5555';
        var email = 'email@email.com';


        //// Act
        scope.address.name = name;
        scope.address.address1 = address1;
        scope.address.address2 = address2;
        scope.address.locality = locality;
        scope.address.region = region;
        scope.address.postalCode = postalCode;
        scope.address.countryCode = countryCode;
        scope.address.organization = organization;
        scope.address.addressType = addressType;
        scope.address.phone = phone;
        scope.address.email = email;

        //// Assert
        expect (scope.dialogData.address.name).toBe(name);
        expect (scope.dialogData.address.address1).toBe(address1);
        expect (scope.dialogData.address.address2).toBe(address2);
        expect (scope.dialogData.address.locality).toBe(locality);
        expect (scope.dialogData.address.region).toBe(region);
        expect (scope.dialogData.address.postalCode).toBe(postalCode);
        expect (scope.dialogData.address.countryCode).toBe(countryCode);
        expect (scope.dialogData.address.organization).toBe(organization);
        expect (scope.dialogData.address.phone).toBe(phone);
        expect (scope.dialogData.address.email).toBe(email);
        expect (scope.dialogData.address.addressType).toBe(addressType);
    });
});