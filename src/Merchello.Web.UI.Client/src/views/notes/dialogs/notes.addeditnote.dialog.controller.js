angular.module('merchello').controller('Merchello.Notes.Dialog.NoteAddEditController',
    ['$scope',
    function($scope) {
        $scope.wasFormSubmitted = false;


        $scope.save = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.addEditNoteForm.message.$valid) {
                $scope.dialogData.note.message = $scope.dialogData.message;
                $scope.submit($scope.dialogData);
            }
        }
}]);
