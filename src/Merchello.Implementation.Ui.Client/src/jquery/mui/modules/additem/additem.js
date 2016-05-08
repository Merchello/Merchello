//// A class to deal with AddItem box JQuery functions
//// This looks for a form with data attribute "data-muifrm='additem'"
MUI.AddItem = {

    init: function() {
        // find all of the AddItem forms
        var frms = $('[data-muifrm="additem"]');
        if (frms.length > 0) {
            $.each(frms, function(idx, frm) {
               MUI.AddItem.bind.form(frm);
            });
        }
    },

    bind: {

        // bind the form;
        form: function(frm) {
            
        }
    }

};
