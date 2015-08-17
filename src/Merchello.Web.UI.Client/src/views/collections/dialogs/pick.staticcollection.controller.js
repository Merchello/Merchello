angular.module('merchello').controller('Merchello.Product.Dialogs.PickStaticCollectionController',
    ['$scope', 'treeService', 'localizationService',
    function($scope, treeService, localizationService) {

        $scope.pickerTitle = '';
        $scope.pickerRootNode = {};

        function init() {
            console.info($scope.dialogData);
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
                if ($scope.pickerRootNode && $scope.pickerRootNode !== undefined) {
                   // exposeChildTree($scope.pickerRootNode);
                }
            });
        }
/*
        function exposeChildTree(node) {
            if (node.hasChildren) {
                treeService.loadNodeChildren({node: node}).then(function (children) {
                    angular.forEach(children, function (child) {
                        exposeChildTree(child);
                        console.info(node);
                    });
                });
            }
        }
*/
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
