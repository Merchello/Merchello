angular.module('merchello').controller('Merchello.Notes.Dialog.NoteAddEditController',
    ['$scope',
    function($scope) {
        $scope.wasFormSubmitted = false;
        $scope.rteProperties = {
            label: 'bodyText',
            view: 'rte',
            config: {
                editor: {
                    toolbar: ["bold", "italic", "bullist", "numlist", "link"],
                    stylesheets: [],
                    dimensions: { height: 350 }
                }
            },
            value: ""
        };

        function init() {
            if ($scope.dialogData.note.message !== '') {
                $scope.rteProperties.value = $scope.dialogData.note.message;
            }
        }

        $scope.save = function() {
            $scope.wasFormSubmitted = true;

            if ($scope.rteProperties.value !== '') {
                $scope.dialogData.note.message = $scope.rteProperties.value;
                $scope.submit($scope.dialogData);
            } else {
                $scope.addEditNoteForm.$valid = false;
            }
        }

        init();
}]);
