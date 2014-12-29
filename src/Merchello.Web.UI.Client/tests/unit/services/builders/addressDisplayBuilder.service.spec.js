'use strict';

    describe("addressDisplayBuilder.service", function () {
        beforeEach(module('umbraco'));

        it ('should be able to build an empty address display', inject(function(addressDisplayBuilder) {
            var address = addressDisplayBuilder.build();

            expect (address).toBeDefined();
        }));

    });
