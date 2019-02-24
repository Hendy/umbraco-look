(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService', '$timeout'];

    function SearcherController($scope, $routeParams, apiService, $q, navigationService, $timeout) {

        // input params
        $scope.searcherName = $routeParams.id;
        $scope.searcherDescription = null;
        $scope.searcherType = null;
        $scope.icon = null;

        // sync tree
        $timeout(function () { // HACK: timeout required as navigationService not ready on inital load
            navigationService.syncTree({
                tree: 'lookTree',
                path: [
                    '-1',
                    'searcher-' + $scope.searcherName
                ],
                forceReload: true
            });
        }, 500);

        // view data
        apiService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;

                $scope.searcherDescription = response.data.SearcherDescription;
                $scope.searcherType = response.data.SearcherType;
                $scope.icon = response.data.Icon;

        });

    }

})();