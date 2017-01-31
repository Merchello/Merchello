'use strict';

function DiploTraceLogDetailController($scope, $routeParams) {

    $("#diplo-logdetail").parent("div").addClass('diplo-tracelog-modal');

    var findInArray = function (array, value, offset) {
        for (var i = 0; i < array.length; i++) {
            if (array[i]["Id"] === value) {
                return array[i + offset];
            }
        }
        return null;
    }

    $scope.getSearchText = function (max) {

        var selection = window.getSelection().toString();

        if (selection.length < 3) {
            selection = $scope.dialogData.logItem.Logger + " " + $scope.dialogData.logItem.Message;
        }

        return selection.substring(0, max);
    }

    $scope.hasPrevious = function () {
        return $scope.dialogData.items[0].Id !== $scope.dialogData.logItem.Id;
    }

    $scope.hasNext = function () {
        return $scope.dialogData.items[$scope.dialogData.items.length - 1].Id !== $scope.dialogData.logItem.Id;
    }

    $scope.nextItem = function () {
        var next = findInArray($scope.dialogData.items, $scope.dialogData.logItem.Id, 1);
        if (next) {
            $scope.dialogData.logItem = next;
        }
    }

    $scope.previousItem = function () {
        var prev = findInArray($scope.dialogData.items, $scope.dialogData.logItem.Id, -1);
        if (prev) {
            $scope.dialogData.logItem = prev;
        }
    }
}