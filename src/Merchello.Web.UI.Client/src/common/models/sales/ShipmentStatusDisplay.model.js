    /**
    * @ngdoc model
    * @name merchello.models.shipmentStatus
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentStatusDisplay object
    */
    var ShipmentStatusDisplay = function () {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('ShipmentStatusDisplay', ShipmentStatusDisplay);