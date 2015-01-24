    /**
     * @ngdoc model
     * @name MerchelloTab
     * @function
     *
     * @description
     * Backoffice model used for tab navigation
     */
    var MerchelloTab = function() {
        var self = this;
        self.id = '';
        self.name = '';
        self.url = '';
        self.active = false;
    };

    angular.module('merchello.models').constant('MerchelloTab', MerchelloTab);

    /**
     * @ngdoc model
     * @name MerchelloTabCollection
     * @function
     *
     * @description
     * Backoffice model used for tab navigation
     */
    var MerchelloTabCollection = function() {
        this.items = [];
    };

    MerchelloTabCollection.prototype = (function() {

        // safely adds a tab to the collection
        function addTab(id, name, url) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.url = url;
                this.items.push(tab);
            }
        }

        function setActive(id) {
           angular.forEach(this.items, function(item) {
               if(item.id === id) {
                   item.active = true;
               } else {
                   item.active = false;
               }
           });
        }

        function insertTab(id, name, url, index) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.url = url;
                if (this.items.length <= index) {
                    addTab.call(this, tab);
                } else {
                    this.items.splice(index, 0, tab);
                }
            }
        }

        /// appends a customer tab to the current collection
        function appendCustomerTab(customerKey) {
            if(customerKey !== '00000000-0000-0000-0000-000000000000') {
                addTab.call(this, 'customer', 'Customer', '#/merchello/merchello/customeroverview/' + customerKey);
            }
        }

        return {
            addTab: addTab,
            setActive: setActive,
            insertTab: insertTab,
            appendCustomerTab: appendCustomerTab
        };
    }());

    angular.module('merchello.models').constant('MerchelloTabCollection', MerchelloTabCollection);