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
                if (label.length > 0) {
                    $(label).html(args.ItemCount);
                }
            }
        }
    }
};
