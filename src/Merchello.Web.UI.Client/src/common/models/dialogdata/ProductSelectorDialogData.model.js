    /**
     * @ngdoc model
     * @name ProductSelectorDialogData
     * @function
     *
     * @description
     * A dialogData model for use in the product selector
     *
     */
    var ProductSelectorDialogData = function() {
        var self = this;
        self.product = {};
    };

    angular.module('merchello.models').constant('ProductSelectorDialogData', ProductSelectorDialogData);
