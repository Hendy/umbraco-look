(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService'];

    function SearcherController($scope, $routeParams, apiService, $q, navigationService) {

        // input params
        $scope.searcherName = $routeParams.id;
        $scope.searcherDescription = null;
        $scope.searcherType = null;
        $scope.icon = null;

        // sync tree
        navigationService.syncTree({
            tree: 'lookTree',
            path: [
                '-1',
                'searcher-' + $scope.searcherName
            ],
            forceReload: true
        });

        // view data
        apiService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) {

                $scope.searcherDescription = response.data.SearcherDescription;
                $scope.searcherType = response.data.SearcherType;
                $scope.icon = response.data.Icon;

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