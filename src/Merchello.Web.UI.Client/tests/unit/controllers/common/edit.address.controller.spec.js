'use strict';

describe("Merchello.Common.Dialogs.EditAddressController", function () {

    var scope, $controllerConstructor;

    beforeEach(module('merchello'));

    beforeEach(inject(function ($rootScope, $controller, addressMocks) {

        $controllerConstructor = $controller;

        var dialogData = {};
        dialogData.address = addressMocks.getSingleAddress();

        scope = $rootScope.$new();
        scope.dialogData = dialogData;

    }));

    it('should set $scope.address to the dialogData address', function() {

        var ctl = $controllerConstructor("Merchello.Common.Dialogs.EditAddressController", { $scope: scope });
      
        expect(scope.address.name).toBe('Disney World');
    });

});