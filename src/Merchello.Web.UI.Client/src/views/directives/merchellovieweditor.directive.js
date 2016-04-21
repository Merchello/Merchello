angular.module('merchello.directives').directive('merchelloViewEditor',
    ['$q', 'assetsService',
    function($q, assetsService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                viewData: '='
            },
            template: '<div class="merchello-codemirror"></div>',

            link: function (scope, elm, attr) {

                scope.loaded = false;

                $q.all([
                    assetsService.loadJs('/umbraco/lib/codemirror/lib/codemirror.js'),
                    assetsService.loadCss('/umbraco/lib/codemirror/lib/codemirror.css')
                ]).then(function() {
                   init();
                });

                var editor;

                function init() {

                    elm.html('');
                    editor = new window.CodeMirror(function(cm) {
                        elm.append(cm);
                    }, {
                        tabMode: "shift",
                        matchBrackets: true,
                        indentUnit: 4,
                        indentWithTabs: true,
                        enterMode: "keep",
                        lineWrapping: false,
                        lineNumbers: true
                    });

                    editor.on('change', function(instance) {
                        scope.viewData.viewBody = instance.getValue();
                    });


                    scope.$watch('viewData', function(newVal, oldVal) {
                        if (newVal !== oldVal) {
                            if (newVal.viewBody !== '') {
                                editor.setValue(newVal.viewBody);
                            }
                        }
                    });

                    scope.loaded = true;
                }
            }
        }
    }]);
