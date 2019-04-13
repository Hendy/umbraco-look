(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.NodeTypeController', NodeTypeController);

    NodeTypeController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function NodeTypeController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.nodeType = parsedId[1];

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'nodeType-' + $scope.searcherName + '|' + $scope.nodeType
        ]);

        // get view data
        apiService.getViewDataForNodeType($scope.searcherName, $scope.nodeType)
            .then(function (response) { $scope.viewData = response.data; });

        // filters
        $scope.filters = {
            nodeType: $scope.nodeType
        }; 

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