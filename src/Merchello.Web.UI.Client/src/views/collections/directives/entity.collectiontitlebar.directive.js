angular.module('merchello.directives').directive('entityCollectionTitleBar', function($compile, localizationService, entityCollectionResource, entityCollectionDisplayBuilder, entityCollectionProviderDisplayBuilder) {
  return {
    restrict: 'E',
    replace: true,
    scope: {
      collectionKey: '=',
      entityType: '='
    },
    template: '<h4>{{ collection.name }}</h4>',
    link: function(scope, element, attrs) {

      scope.collection = {};

      function init() {
        scope.$watch('collectionKey', function(newValue, oldValue) {
          loadCollection();
        });
      }

      function loadCollection() {
        if(scope.collectionKey === 'manage' || scope.collectionKey === '' || scope.collectionKey === undefined) {
          var key = 'merchelloCollections_all' + scope.entityType;
          localizationService.localize(key).then(function (value) {
            scope.collection.name = value;
          });
        } else {
          entityCollectionResource.getByKey(scope.collectionKey).then(function(collection) {
            var retrieved = entityCollectionDisplayBuilder.transform(collection);
            entityCollectionResource.getEntityCollectionProviders().then(function(results) {
              var providers = entityCollectionProviderDisplayBuilder.transform(results);
              var provider = _.find(providers, function(p) {
                return p.key === retrieved.providerKey;
              });
              if (provider !== undefined && provider.managesUniqueCollection && provider.localizedNameKey !== '') {
                localizationService.localize(provider.localizedNameKey.replace('/', '_')).then(function(value) {
                  scope.collection.name = value;
                });
              } else {
                scope.collection = retrieved;
              }
            });
          });
        }
      }

      init();
    }
  }
});
