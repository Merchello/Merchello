//// A class to deal with basket JQuery functions
//// This looks for a form with data attribute "data-muifrm='wishlist'"
MUI.WishList = {
    
    init: function() {
        if (MUI.Settings.Endpoints.wishListSurface === undefined || MUI.Settings.Endpoints.wishListSurface === '') return;

        var frm = $('[data-muifrm="wishlist"]');
        if (frm.length > 0) {
            MUI.WishList.bind.form(frm[0]);
        }
    },

    bind: {

        form: function(frm) {
            // Watch for changes in the input fields
            $(frm).find(':input[data-muiaction="updatequantity"]').change(function() {
                var frmRef = $(this).closest('form');

                // post the form to update the basket quantities
                var url = MUI.Settings.Endpoints.wishListSurface + 'UpdateWishList';
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: $(frmRef).serialize()
                }).then(function(results) {

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
    
}
