(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SortController', SortController);

    SortController.$inject = ['$scope', 'Look.BackOffice.SortService'];

    function SortController($scope, sortService) {

        $scope.sortOptions = [];

        $scope.sortOn = sortService.sortOn;

        // TODO: only enable if there are items in the index
        $scope.sortOptions.push({ value: 'Score', label: 'Score', disabled: false }); // default sort order
        $scope.sortOptions.push({ value: 'Name', label: 'Name', disabled: false });

        // add later after 0.1.0
        //$scope.sortOptions.push({ value: 'DateAscending', label: 'Date Ascending', disabled: true });
        //$scope.sortOptions.push({ value: 'DateDecending', label: 'Date Decending', disabled: true });

        $scope.change = function () {
            sortService.change($scope.sortOn);
        };

    }

})();