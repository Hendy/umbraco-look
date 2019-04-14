﻿(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagsController', TagsController);

    TagsController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function TagsController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'tags-' + $scope.searcherName
        ]);

        // view data
        apiService.getViewDataForTags($scope.searcherName)
            .then(function (response) { $scope.viewData = response.data; });

        // filters
        $scope.getFilters = function () {

            var q = $q.defer();

            apiService
                .getTagFilters($scope.searcherName, undefined, undefined)
                .then(function (response) {
                    q.resolve(response.data);
                });

            return q.promise;
        };

        // matches
        $scope.getMatches = function (filter, sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, undefined, undefined, filter, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

    }

})();