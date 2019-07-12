//// A class to assist in updating labels in the UI
//// This looks for a form with data attribute "data-muilabel='VALUE'"
MUI.Labels = {
    
    init: function() {
        // Event listener
        MUI.on('AddItem.added', MUI.Labels.update.basketItemCount);
    },
    
    update: {
        
        basketItemCount: function(evt, args) {
            if (args.Success) {
                var label = $('[data-muilabel="basketcount"]');
                var $quickcheckout = $('[data-value="quickcheckout"]');
                if (label.length > 0) {
                    $(label).html(args.ItemCount);
                    // If there's a quick checkout menu item show or hide it according to basket count.
                    if ($quickcheckout.length > 0) {
                        if (args.ItemCount !== 0) {
                            $quickcheckout.removeClass('mui-quickcheckout');
                        }
                        else {
                            $quickcheckout.addClass('mui-quickcheckout');
                        }
                    }
                }

            }
        }
    }
};
