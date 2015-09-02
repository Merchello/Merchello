angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductListViewDialogController',
    ['$scope', 'treeService', 'localizationService', 'eventsService',
    function($scope, treeService, localizationService, eventsService) {

        $scope.pickerRootNode = {};
        $scope.allowMultiple = false;
        $scope.getTreeId = getTreeId;
        $scope.hasSelection = hasSelection;
        $scope.setAllProducts = setAllProducts;

        var eventName = 'merchello.entitycollection.selected';

        function init() {
            eventsService.on(eventName, onEntityCollectionSelected);
            $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            setTitle();
        }

        function setTitle() {
            var key = 'merchelloCollections_' + $scope.dialogData.entityType + 'Collections';
            localizationService.localize(key).then(function (value) {
                $scope.pickerTitle = value;
                setTree();
            });
        }

        function setTree() {
            treeService.getTree({section: 'merchello'}).then(function(tree) {
                var root = tree.root;
                var treeId = getTreeId();
                $scope.pickerRootNode = _.find(root.children, function (child) {
                    return child.id === treeId;
                });
            });
        }

        function setAllProducts() {
            if (!hasSelection()) {
                $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            }
        }

        function onEntityCollectionSelected(eventName, args, ev) {
            //  {key: "addCollection", value: "4d026d91-fe13-49c7-8f06-da3d9f012181"}
            if (args.key === 'addCollection') {
               $scope.dialogData.collectionKey = args.value;
                $('#all-items i.icon').removeClass('icon-check').addClass('icon-add');
            }
            if (args.key === 'removeCollection') {
                $scope.dialogData.collectionKey = '';
                $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            }
        }

        function hasSelection() {
            return $scope.dialogData.collectionKey !== '';
        }

        function getTreeId() {
            return "products";
        }

        init();
}]);
