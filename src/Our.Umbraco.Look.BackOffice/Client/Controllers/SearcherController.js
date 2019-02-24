(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', 'navigationService', '$timeout'];

    function SearcherController($scope, $routeParams, apiService, navigationService, $timeout) {

        // input params
        $scope.searcherName = $routeParams.id;

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
            .then(function (response) { $scope.viewData = response.data; });

    }

})();