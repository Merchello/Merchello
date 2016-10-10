angular.module('merchello').controller('Merchello.Product.Dialogs.PickStaticCollectionController',
    ['$scope', 'eventsService', 'treeService', 'localizationService',
    function($scope, eventsService, treeService, localizationService) {

        $scope.pickerTitle = '';

        $scope.getTreeId = getTreeId;

        var eventName = 'merchello.entitycollection.selected';

        function init() {
            eventsService.on(eventName, onEntityCollectionSelected);
            setTitle();
        }

        function onEntityCollectionSelected(eventName, args, ev) {
            //  {key: "addCollection", value: "4d026d91-fe13-49c7-8f06-da3d9f012181"}
            if (args.key === 'addCollection') {
                $scope.dialogData.addCollectionKey(args.value);
            }
            if (args.key === 'removeCollection') {
                $scope.dialogData.removeCollectionKey(args.value);
            }
        }


        function setTitle() {
            var key = 'merchelloCollections_' + $scope.dialogData.entityType.toLowerCase() + 'Collections';
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

        function getTreeId() {
            switch ($scope.dialogData.entityType) {
                case 'product':
                    return 'products';
                case 'invoice':
                    return 'sales';
                case 'customer':
                    return 'customers';
                default:
                    return '';
            }
        }

        // intitialize
        init();
}]);
