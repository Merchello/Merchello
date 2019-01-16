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
        self.visible = true;
        self.callback = undefined;
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

        function addActionTab(id, name, callback) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.callback = callback;
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

        function hideTab(id) {
            var existing = _.find(this.items, function(tab) {return tab.id === id; });
            if (existing !== undefined && existing !== null) {
                existing.visible = false;
            }
        }

        function showTab(id) {
            var existing = _.find(this.items, function(tab) {return tab.id === id; });
            if (existing !== undefined && existing !== null) {
                existing.visible = true;
            }
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

        function insertActionTab(id, name, callback, index) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.callback = callback;
                if (this.items.length <= index) {
                    addTab.call(this, tab);
                } else {
                    this.items.splice(index, 0, tab);
                }
            }
        }


        // appends a customer tab to the current collection
        function appendCustomerTab(customerKey) {
            if (customerKey !== '00000000-0000-0000-0000-000000000000' && customerKey !== null && customerKey !== undefined) {
                addTab.call(this, 'customer', 'merchelloTabs_customer', '#/merchello/merchello/customeroverview/' + customerKey);
            }
        }

        function appendOfferTab(offerKey, backOfficeTree) {
            var title = 'merchelloTabs_';
            if(backOfficeTree.title === undefined || backOfficeTree.title === '') {
                title += 'offer';
            } else {
                title += backOfficeTree.title.toLowerCase();
            }
            if(offerKey !== '00000000-0000-0000-0000-000000000000' && offerKey !== 'create') {
                addTab.call(this, 'offer', title, '#' + backOfficeTree.routePath.replace('{0}', offerKey));
            } else {
                addTab.call(this, 'offer', title, '#' +backOfficeTree.routePath.replace('{0}', 'create'));
            }
        }

        return {
            addTab: addTab,
            setActive: setActive,
            insertTab: insertTab,
            appendCustomerTab: appendCustomerTab,
            appendOfferTab: appendOfferTab,
            addActionTab: addActionTab,
            insertActionTab: insertActionTab,
            hideTab: hideTab,
            showTab: showTab
        };
    }());

    angular.module('merchello.models').constant('MerchelloTabCollection', MerchelloTabCollection);