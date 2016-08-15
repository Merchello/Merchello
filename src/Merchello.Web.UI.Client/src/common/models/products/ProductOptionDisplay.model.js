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
        self.useName = '';
        self.uiOption = '';
        self.required = true;
        self.shared = false;
        self.sharedCount = 0;
        self.sortOrder = 1;
        self.detachedContentTypeKey = '';
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

        // removes the product options choice
        function removeAttributeChoice(idx) {
            if(idx >= 0) {
               this.choices.splice(idx, 1);
            }
        }

        function hasDetachedContent() {
            return this.detachedContentTypeKey !== '00000000-0000-0000-0000-000000000000';
        }

        function canBeDeleted() {
            return this.shared ? this.shareCount === 0 : true;
        }

        // resets the product options choice sort order
        function resetChoiceSortOrders() {
            for (var i = 0; i < this.choices.length; i++) {
                this.choices[i].sortOrder = i + 1;
            }
        }

        return {
            addAttributeChoice: addAttributeChoice,
            removeAttributeChoice: removeAttributeChoice,
            resetChoiceSortOrders: resetChoiceSortOrders,
            hasDetachedContent: hasDetachedContent,
            canBeDeleted: canBeDeleted
        };
    }());

    angular.module('merchello.models').constant('ProductOptionDisplay', ProductOptionDisplay);