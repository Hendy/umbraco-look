﻿(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.DetachedController', DetachedController);

    DetachedController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function DetachedController($scope, $routeParams, $q, apiService, treeService) {
        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.nodeType = parsedId[1];

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'nodeType-' + $scope.searcherName + '|' + $scope.nodeType, // (this whole tree path patttern needs to be refactored)
            'detached-' + $scope.searcherName + '|' + $scope.nodeType
        ]);

        // view data
        apiService.getViewDataForDetached($scope.searcherName, $scope.nodeType)
            .then(function (response) { $scope.viewData = response.data; });

        // filters
        $scope.getFilters = function () {

            var q = $q.defer();

            apiService
                .getDetachedFilters($scope.searcherName, $scope.nodeType)
                .then(function (response) {
                    q.resolve(response.data);
                });

            return q.promise;
        };

        // matches
        $scope.getMatches = function (filter, sort, skip, take) {

            var q = $q.defer();

            apiService
                .getDetachedMatches($scope.searcherName, $scope.nodeType, filter, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

    }

})();