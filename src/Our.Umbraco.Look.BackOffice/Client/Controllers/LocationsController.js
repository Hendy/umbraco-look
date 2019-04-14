﻿(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.LocationsController', LocationsController);

    LocationsController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function LocationsController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'locations-' + $scope.searcherName
        ]);

        // view data
        apiService.getViewDataForLocations($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;
            });

        // filters
        $scope.getFilters = function () {

            var q = $q.defer();

            apiService
                .getLocationFilters($scope.searcherName)
                .then(function (response) {
                    q.resolve(response.data);
                });

            return q.promise;
        };

        // matches
        $scope.getMatches = function (filter, sort, skip, take) {

            var q = $q.defer();

            apiService
                .getLocationMatches($scope.searcherName, filter, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();