(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.FiltersController', FiltersController);

    FiltersController.$inject = ['$scope'];

    function FiltersController($scope) {

        $scope.debug = "filters";

    }

})();