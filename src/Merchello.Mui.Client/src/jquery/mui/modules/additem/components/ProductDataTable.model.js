//// Cart model
MUI.AddItem.ProductDataTable = function() {
    var self = this;
    self.productKey = '';
    self.rows = [];
};

/// Cart Model Prototypes
//  ProductDataTable
MUI.AddItem.ProductDataTable.prototype = (function() {

    // used to match a pricing row with the drop down selection
    function getRowByMatchKeys(keys) {
        var row = _.find(this.rows, function (r) {
            var test = _.intersection(r.matchKeys, keys);
            return test.length === keys.length;
        });
        return row;
    }

    return {
        getRowByMatchKeys: getRowByMatchKeys
    };

}());
