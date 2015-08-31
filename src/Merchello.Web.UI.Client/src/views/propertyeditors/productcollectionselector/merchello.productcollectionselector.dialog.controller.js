angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductCollectionSelectorDialogController',
    ['$scope', 'treeService', 'localizationService',
    function($scope, treeService, localizationService) {

        $scope.pickerRootNode = {};
        $scope.allowMultiple = false;
        $scope.getTreeId = getTreeId;

        function init() {
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
                console.info(root);
                $scope.pickerRootNode = _.find(root.children, function (child) {
                    return child.id === treeId;
                });
            });
        }

        function getTreeId() {
            return "products";
        }

        init();
}]);
