angular.module("umbraco")
	.controller("Merchello.MerchelloController",
		function ($scope, assetsService) {
            assetsService.
			assetsService.load([
				'/App_Plugins/Merchello/lib/bootstrap/js/bootstrap.min.js',
				'/App_Plugins/Merchello/lib/select2/ui-select2.js',
				'/App_Plugins/Merchello/lib/ngTagsInput/ng-tags-input.min.js',
				'/App_Plugins/Merchello/Common/Js/merchello.namespaces.js',
				'/App_Plugins/Merchello/Modules/Catalog/product.models.js',
				'/App_Plugins/Merchello/Directives/tag.manager.directive.js',
				'/App_Plugins/Merchello/Modules/Catalog/merchello.product.service.js',
				'/App_Plugins/Merchello/Modules/Catalog/merchello.productvariant.service.js',
				'/App_Plugins/Merchello/Modules/Catalog/product.list.controller.js',
				'/App_Plugins/Merchello/Modules/Catalog/product.create.controller.js',
				'/App_Plugins/Merchello/Modules/Catalog/product.edit.controller.js',
				'/App_Plugins/Merchello/Modules/Catalog/productvariant.edit.controller.js'
			])
				.then(function () {
					alert("editor dependencies loaded");
				});

			assetsService.loadCss('/App_Plugins/Merchello/Common/Css/merchello.css');
			assetsService.loadCss('/App_Plugins/Merchello/lib/ngTagsInput/ng-tags-input.min.css');

		});