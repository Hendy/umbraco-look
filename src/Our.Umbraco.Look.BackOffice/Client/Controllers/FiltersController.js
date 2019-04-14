(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.FiltersController', FiltersController);

    FiltersController.$inject = ['$scope', 'Look.BackOffice.FiltersService'];

    function FiltersController($scope, filtersService) {

        filtersService.filterAlias = undefined;

        $scope.filterAlias = "";

        $scope.filters = {};

        $scope.$parent.getFilters().then(function (filters) {
            $scope.filters = filters;
        });

        $scope.changeAlias = function () {
            filtersService.change($scope.filterAlias);         
        };
    }

})();