MUI.Checkout.Address = {
  
    addressType: '',
    
    init: function() {
        if (MUI.Settings.Endpoints.countryRegionApi === undefined || MUI.Settings.Endpoints.countryRegionApi === '') return;

        var frm = $('[data-muistage="BillingAddress"]');

        if (frm.length > 0) {
            MUI.Checkout.Address.addressType = "Billing";
            MUI.Checkout.Address.bind.form(frm[0]);
            var countryddl = frm.find(':input[data-muiaction="populateregion"]');
            //if country not selected, set to default
            if (countryddl.val().length == 0) {
                if (MUI.Settings.Defaults.BillingCountryCode.length > 0) {
                    countryddl.val(MUI.Settings.Defaults.BillingCountryCode);
                }
            }
            MUI.Checkout.Address.populateRegion(countryddl);

        } else {
            frm = $('[data-muistage="ShippingAddress"]');
            if (frm.length > 0) {
                MUI.Checkout.Address.addressType = "Shipping";
                MUI.Checkout.Address.bind.form(frm[0]);
                var countryddl = frm.find(':input[data-muiaction="populateregion"]');
                //if country not selected, set to default
                if (countryddl.val().length == 0) {
                    if (MUI.Settings.Defaults.ShippingCountryCode.length > 0) {
                        countryddl.val(MUI.Settings.Defaults.ShippingCountryCode);
                    }
                }
                MUI.Checkout.Address.populateRegion(countryddl);
            }
        }
    },

    bind: {

        form: function (frm) {
            // Watch for changes in the input fields
            $(frm).find(':input[data-muiaction="populateregion"]').change(function () {
                MUI.Checkout.Address.populateRegion($(this));
            });

            $(frm).find(':input[data-muiaction="updateregion"]').change(function () {
                var frmRef = $(this).closest('form');
                //keep the select list and the region textbox in sync
                frmRef.find(':input[data-muivalue="region"]').val(frmRef.find(':input[data-muiaction="updateregion"]').val());
            });
        }
    },

    populateRegion: function (countryCode) {
        var frmRef = countryCode.closest('form');
        // post the country to get the regions for that country
        var url = MUI.Settings.Endpoints.countryRegionApi + 'PostGetRegionsForCountry?countryCode=' + countryCode.val();
        $.ajax({
            type: 'POST',
            url: url
        }).then(function (results) {
            var regionddl = frmRef.find(':input[data-muiaction="updateregion"]');
            var regiontb = frmRef.find(':input[data-muivalue="region"]');
            if (results.length > 0) {
                //remove all but the first option
                $('option', regionddl).not(':eq(0)').remove();
                $.each(results, function (idx, item) {
                    regionddl.append($("<option></option>").attr("value", item.code).text(item.name));
                });
                //if region already defined and is present in the drop down, select it
                if (regiontb.val().length > 0 && regionddl.find('option[value="' + regiontb.val() + '"]').length > 0) {//and it exists in the drop down) {
                    //set ddl to current value
                    regionddl.val(regiontb.val());
                }
                else {
                    regiontb.val('');
                }
                //show region ddl and hide region textbox
                regionddl.show();
                regiontb.hide();
                //$('#UseForShipping').attr('disabled', false);
            }
            else {
                //hide region ddl and show region textbox
                regionddl.hide();
                regiontb.val('').show();
                //also, if there is no region then billing and shipping must be different??
                //$('#UseForShipping').prop('checked', false).attr('disabled', true);
            }
        }, function (err) {
            MUI.Logger.captureError(err);
        });
    }
    
    
};
