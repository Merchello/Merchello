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

            // Watch for changes in the input fields
            $(frm).find(':input').change(function() {
                var frmRef = $(this).closest('form');

                // post the form to update the basket quantities
                MUI.Forms.post(frmRef, MUI.Settings.basketSurfaceEndpoint + 'UpdateBasket')
                    .then(function(results) {

                        // update the line items sub totals
                        $.each(results.UpdatedItems, function(idx, item) {
                            var hid = $('input:hidden[value="' + item.Key + '"]')
                            if (hid.length > 0) {
                                var subtotal = $(hid).closest('tr').find('[data-muivalue="linetotal"]');
                                if (subtotal.length > 0) {
                                    $(subtotal).html(item.FormattedTotal);
                                }
                            }
                        });

                        // set the new basket total
                        var total = $(frmRef).find('[data-muivalue="total"]');
                        if (total.length > 0) {
                            $(total).html(results.FormattedTotal);
                        }

                    }, function(err) {
                       MUI.Logger.captureError(err);
                    });
            });


            var btn = $(frm).find('[data-muibtn="update"]');
            if (btn.length > 0) {
                $(btn).hide();
            }

        }
    }
};
