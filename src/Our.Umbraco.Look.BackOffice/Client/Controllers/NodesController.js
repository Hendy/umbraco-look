(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.NodesController', NodesController);

    NodesController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService', '$timeout'];

    function NodesController($scope, $routeParams, apiService, $q, navigationService, $timeout) {

        // input params
        $scope.searcherName = $routeParams.id;

        // sync tree
        $timeout(function () { // HACK: timeout required as navigationService not ready on inital load
            navigationService.syncTree({
                tree: 'lookTree',
                path: [
                    '-1',
                    'searcher-' + $scope.searcherName,
                    'nodes-' + $scope.searcherName
                ],
                forceReload: true
            });
        }, 500);

        // view data
        apiService.getViewDataForNodes($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;
            });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getMatches($scope.searcherName, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();