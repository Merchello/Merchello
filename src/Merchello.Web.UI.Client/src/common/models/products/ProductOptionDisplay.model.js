    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductOptionDisplay object
     */
    var ProductOptionDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.required = true;
        self.sortOrder = 1;
        self.choices = [];
    };

    ProductOptionDisplay.prototype = (function() {

        function addAttributeChoice(choiceName) {
            var attribute = new ProductAttributeDisplay();
            attribute.name = choiceName;
            attribute.sortOrder = this.choices.length + 1;
            // TODO skus
            this.choices.push(attribute);
        }

        return {
            addAttributeChoice: addAttributeChoice
        };
    }());

    angular.module('merchello.models').constant('ProductOptionDisplay', ProductOptionDisplay);