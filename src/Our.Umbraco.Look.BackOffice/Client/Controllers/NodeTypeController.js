(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.NodeTypeController', NodeTypeController);

    NodeTypeController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService', '$timeout'];

    function NodeTypeController($scope, $routeParams, apiService, $q, navigationService, $timeout) {

        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.nodeType = parsedId[1];

        // sync tree
        $timeout(function () { // HACK: timeout required as navigationService not ready on inital load
            navigationService.syncTree({
                tree: 'lookTree',
                path: [
                    '-1',
                    'searcher-' + $scope.searcherName,
                    'nodes-' + $scope.searcherName,
                    'nodeType-' + $scope.searcherName + '|' + $scope.nodeType
                ],
                forceReload: true
            });
        }, 500);

        // get view data
        apiService.getViewDataForNodeType($scope.searcherName, $scope.nodeType)
            .then(function (response) { $scope.viewData = response.data; });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getNodeTypeMatches($scope.searcherName, $scope.nodeType, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();