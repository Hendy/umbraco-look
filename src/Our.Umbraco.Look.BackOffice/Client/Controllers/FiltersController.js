(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.FiltersController', FiltersController);

    FiltersController.$inject = ['$scope', 'Look.BackOffice.ApiService', 'Look.BackOffice.FiltersService'];

    function FiltersController($scope, apiService, filtersService) {

        $scope.debug = "filters";

        $scope.filterAlias = null;

        $scope.filters = {};
        $scope.filters.aliasOptions = [];
        //$scope.filters.aliasOptions.push({ value: '', label: '', disabled: false });
        //$scope.filters.aliasOptions.push({ value: 'xxx', label: 'xxx', disabled: false });

        //var getFilters = $scope.$parent.getFilters; // expected to exist

        // call parent to get filter optoins (all aliases in use for current tree view node query type)
        $scope.$parent.getFilters().then(function (filters) {
            $scope.filters = filters;

        });

        $scope.changeAlias = function () {
            filtersService.change($scope.filterAlias);         
        };

    }

})();