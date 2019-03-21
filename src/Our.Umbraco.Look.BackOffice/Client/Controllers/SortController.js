(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SortController', SortController);

    SortController.$inject = ['$scope', 'Look.BackOffice.SortService'];

    function SortController($scope, sortService) {

        $scope.sortOptions = [];

        $scope.sortOn = sortService.sortOn;

        // TODO: only enable if there are items in the index (everything should have a name & date + a result score)
        $scope.sortOptions.push({ value: 'Score', label: 'Score', disabled: false }); // default sort order
        $scope.sortOptions.push({ value: 'Name', label: 'Name', disabled: false });
        $scope.sortOptions.push({ value: 'Date', label: 'Date', disabled: false });
        //$scope.sortOptions.push({ value: 'DateAscending', label: 'Date (Oldest First)', disabled: true });

        $scope.change = function () {
            sortService.change($scope.sortOn);
        };

    }

})();