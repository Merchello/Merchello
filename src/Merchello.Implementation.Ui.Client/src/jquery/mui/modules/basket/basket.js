//// A class to deal with basket JQuery functions
//// This looks for a form with data attribute "data-muifrm='basket'"
MUI.Basket = {

    // initialize the basket
    init: function() {
        if (MUI.Settings === undefined) return;
        if (MUI.Settings.basketSurfaceEndpoint === '') return;

        var frm = $('[data-muifrm="basket"]');
        if (frm.length > 0) {
            MUI.Basket.bind.form(frm[0]);
        }
    },

    // binds the form
    bind: {
        form: function(frm) {

            $(frm).find(':input').change(function() {
                var frmRef = $(this).closest('form');
                $.ajax({
                    type: 'POST',
                    url: MUI.Settings.basketSurfaceEndpoint + 'UpdateBasket',
                    data: $(frmRef).serialize(),
                    success: function(data) {
                        console.info(data);
                    }
                });
            });


            var btn = $(frm).find('[data-muibtn="update"]');
            if (btn.length > 0) {
                $(btn).hide();
            }

        }
    }
};
