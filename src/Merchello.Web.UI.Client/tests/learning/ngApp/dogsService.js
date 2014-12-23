(function () {
    angular.module('services').factory('Dog', function () {

        var dogs = [
            { type: "Labrador", name: "Rover" },
            { type: "Tibetan Terrier", name: "Joey" },
            { type: "Yorkshire Terrier", name: "Rufus" }
        ];

        return {
            query: function () {
                return dogs;
            },
            add: function (dog) {
                dogs.push(dog);
            }
        };
    });
}());