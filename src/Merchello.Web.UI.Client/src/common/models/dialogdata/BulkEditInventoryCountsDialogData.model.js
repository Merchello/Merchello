    /**
     * @ngdoc model
     * @name BulkEditInventoryCountsDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for bulk editing of inventory counts.
     */
    var BulkEditInventoryCountsDialogData = function() {
        var self = this;
        self.count = 0;
        self.includeLowCount = false;
        self.lowCount = 0;
        self.warning = '';
    };

    angular.module('merchello.models').constant('BulkEditInventoryCountsDialogData', BulkEditInventoryCountsDialogData);
