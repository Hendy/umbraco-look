(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function SearcherController($scope, $routeParams, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName
        ]);

        // view data
        apiService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) { $scope.viewData = response.data; });
    }

})();