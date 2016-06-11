/**
 * @ngdoc service
 * @name detachedContentTypeDisplayBuilder
 *
 * @description
 * A utility service that builds DetachedContentTypeDisplay models
 */
angular.module('merchello.models').factory('detachedContentTypeDisplayBuilder',
	['genericModelBuilder', 'typeFieldDisplayBuilder', 'umbContentTypeDisplayBuilder', 'DetachedContentTypeDisplay',
	function (genericModelBuilder, typeFieldDisplayBuilder, umbContentTypeDisplayBuilder, DetachedContentTypeDisplay) {

		var Constructor = DetachedContentTypeDisplay;

		return {
			createDefault: function () {
				var content = new Constructor();
				content.entityTypeField = typeFieldDisplayBuilder.createDefault();
				content.umbContentType = umbContentTypeDisplayBuilder.createDefault();
			    return content;
			},
			transform: function (jsonResult) {
				var contents = [];
				if (angular.isArray(jsonResult)) {
					for(var i = 0; i < jsonResult.length; i++) {
						var content = genericModelBuilder.transform(jsonResult[ i ], Constructor);
						content.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].entityTypeField);
						content.umbContentType = umbContentTypeDisplayBuilder.transform(jsonResult[ i ].umbContentType);
						contents.push(content);
					}
				} else {
				    contents = genericModelBuilder.transform(jsonResult, Constructor);
					contents.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField);
					contents.umbContentType = umbContentTypeDisplayBuilder.transform(jsonResult.umbContentType);
				}
				return contents;
			}
		};
	}
]);