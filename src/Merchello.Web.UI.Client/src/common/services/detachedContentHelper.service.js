angular.module('merchello.services').factory('detachedContentHelper',
    ['$q', 'fileManager', 'formHelper',
    function($q, fileManager, formHelper) {

        function validate(args) {
            if (!angular.isObject(args)) {
                throw "args must be an object";
            }
            if (!args.scope) {
                throw "args.scope is not defined";
            }
            if (!args.content) {
                throw "args.content is not defined";
            }
            if (!args.statusMessage) {
                throw "args.statusMessage is not defined";
            }
            if (!args.saveMethod) {
                throw "args.saveMethod is not defined";
            }
        }

        return {

            attributeContentPerformSave: function(args) {
                validate(args);

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // get any files from the fileManager
                    var files = fileManager.getFiles();

                    angular.forEach(args.scope.contentTabs, function(ct) {
                        angular.forEach(ct.properties, function (p) {
                            if (typeof p.value !== "function") {
                                args.scope.dialogData.choice.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                            }
                        });
                    });

                    args.saveMethod(args, files).then(function(data) {

                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });
                        args.scope.preValuesLoaded = true;
                        deferred.resolve(data);

                    }, function (err) {

                        args.scope.preValuesLoaded = true;
                        deferred.reject(err);
                    });

                } else {
                    deferred.reject();
                }

                return deferred.promise;
            },

            detachedContentPerformSave: function(args) {

                validate(args);

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // get any files from the fileManager
                    var files = fileManager.getFiles();

                    // save the current language only
                    angular.forEach(args.scope.contentTabs, function(ct) {
                        if (ct.id === 'render') {
                            args.scope.detachedContent.slug = _.find(ct.properties, function(s) { return s.alias === 'slug'; }).value;
                            args.scope.detachedContent.templateId = _.find(ct.properties, function(t) { return t.alias === 'templateId'; }).value;
                            args.scope.detachedContent.canBeRendered = _.find(ct.properties, function(r) { return r.alias === 'canBeRendered'; }).value === '1' ? true : false;
                        } else {
                            angular.forEach(ct.properties, function (p) {
                                if (typeof p.value !== "function") {
                                    args.scope.detachedContent.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                                }
                            });
                        }
                    });

                    //args.saveMethod(args.content, args.scope.language.isoCode, files)
                    args.saveMethod(args, files).then(function(data) {
                        
                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });
                        args.scope.preValuesLoaded = true;
                        deferred.resolve(data);

                    }, function (err) {

                        args.scope.preValuesLoaded = true;
                        deferred.reject(err);
                    });
                } else {
                    deferred.reject();
                }

                return deferred.promise;
            },

            buildRenderTab: function(args) {
                var items = [];
                var i = 1;
                _.each(args.allowedTemplates, function(t) {
                  items.push({ id: t.id, sortOrder: i, value: t.name });
                    i++;
                });

                var tab = {
                    alias: args.tabAlias,
                    label: args.tabLabel,
                    id: args.tabId,
                    properties: [
                        {
                            alias: 'slug',
                            description: args.slugDescription,
                            editor: 'Umbraco.Textbox',
                            hideLabel: false,
                            label: args.slugLabel,
                            validation: {
                                mandatory: true
                            },
                            value: args.slug,
                            view: 'textbox'
                        },
                        {
                            alias: 'templateId',
                            editor: 'Umbraco.DropDown',
                            hideLabel: false,
                            label: args.templateLabel,
                            config: {
                                items: items
                            },
                            description: '',
                            value: args.templateId === 0 ? args.defaultTemplateId : args.templateId,
                            validation: {
                                mandatory: false
                            },
                            view: 'dropdown'
                        },
                        {
                            alias: 'canBeRendered',
                            editor: 'Umbraco.TrueFalse',
                            description: '',
                            label: args.canBeRenderedLabel,
                            hideLabel: false,
                            value: args.canBeRendered ? '1' : '0',
                            view: 'boolean',
                            validation: {
                                mandatory: false
                            }
                        }
                    ]
                };

                return tab;
            }

        };

}]);
