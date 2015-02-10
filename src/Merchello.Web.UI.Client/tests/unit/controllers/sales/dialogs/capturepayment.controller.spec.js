'use strict';

describe('capturepayment.controller', function() {

    var scope, controller;

    beforeEach(module('umbraco'));

    beforeEach(inject(function ($rootScope, $controller, dialogDataMocks) {
        var dialogData = dialogDataMocks.getCapturePaymentDialogData();
        scope = $rootScope.$new();
        scope.dialogData = dialogData;
        controller = $controller('Merchello.Sales.Dialog.CapturePaymentController', { $scope: scope });
    }));

});
