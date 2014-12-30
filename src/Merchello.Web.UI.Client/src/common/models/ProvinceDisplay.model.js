    /**
     * @ngdoc model
     * @name ProvinceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProvinceDisplay object
     */
     var ProvinceDisplay = function()
     {
         var self = this;
         self.name = '';
         self.code = '';
     };

    angular.module('merchello.models').constant('ProvinceDisplay', ProvinceDisplay);