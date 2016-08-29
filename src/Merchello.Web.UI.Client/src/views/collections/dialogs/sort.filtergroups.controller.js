angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SortFilterGroupsController',
['$scope',
   function($scope) {


       $scope.sortableCollections = {
           start : function(e, ui) {
               ui.item.data('start', ui.item.index());
           },
           stop: function (e, ui) {
               var start = ui.item.data('start'),
                   end =  ui.item.index();
               for(var i = 0; i < $scope.dialogData.collections.length; i++) {
                   $scope.dialogData.collections[i].sortOrder = i;
               }
           },
           disabled: false,
           cursor: "move"
       }

   }]);
