(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SortController', SortController);

    SortController.$inject = ['$scope', 'Look.BackOffice.SortService'];

    function SortController($scope, sortService) {

        $scope.sortOptions = ['Score']; // everything can be sorted by score

        $scope.sortOptions.push('Name');
        $scope.sortOptions.push('Date');

        $scope.change = function () {
            sortService.sortOn = $scope.sortOn; // set value for service consumers
        };

    }

})();