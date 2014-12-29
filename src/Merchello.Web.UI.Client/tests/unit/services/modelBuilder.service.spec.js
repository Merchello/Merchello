'use strict';

describe('modelBuilder.service', function() {

    beforeEach(module('umbraco'));

    it ('should be possible to build a TypeField from the prototype definition', inject(function(TypeFieldDisplay) {
        var tf = new TypeFieldDisplay();

        expect (tf.alias).toBe('test');

    }));
});
