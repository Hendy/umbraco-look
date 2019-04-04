(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.NodesController', NodesController);

    NodesController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function NodesController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'nodes-' + $scope.searcherName
        ]);

        // view data
        apiService.getViewDataForNodes($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;
            });

        // filters
        $scope.filters = {}; 

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