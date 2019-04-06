(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function SearcherController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName
        ]);

        // view data
        apiService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) { $scope.viewData = response.data; });

        // matches - WIRE THIS UP
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