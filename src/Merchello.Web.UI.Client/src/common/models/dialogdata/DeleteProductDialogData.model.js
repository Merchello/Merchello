    /**
     * @ngdoc model
     * @name DeleteProductDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting products methods.
     */
    var DeleteProductDialogData = function() {
        var self = this;
        self.product = {};
        self.name = '';
        self.waring = '';
    };

    angular.module('merchello.models').constant('DeleteProductDialogData', DeleteProductDialogData);
