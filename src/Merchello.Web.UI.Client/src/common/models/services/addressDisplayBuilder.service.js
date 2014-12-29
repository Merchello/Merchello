
    angular.module('merchello.models')
        .factory('addressDisplayBuilder',
            [AddressDisplay, genericModelBuilder,
                function() {

                var Constructor = AddressDisplay;

                function getConstructor()
                {
                    return Constructor;
                }

                function build() {
                    return new Constructor();
                }

                function transform(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }

                return {
                    getConstructor : getConstructor,
                    build: build,
                    transform: transform
                };
        }]);
