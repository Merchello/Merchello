MUI.Assets = {

    // gets a script ensuring it is only ever loaded once.
    // currently used in loading Braintree scripts
    // https://learn.jquery.com/code-organization/deferreds/examples/
    cachedGetScript: MUI.createCache(function(defer, url) {
        $.getScript( url ).then( defer.resolve, defer.reject );
    })
    
};
