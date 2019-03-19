(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.CulturesController', CulturesController);

    CulturesController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function CulturesController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        $scope.searcherName = $routeParams.id;

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'nodes-' + $scope.searcherName,
            'nodeType-' + $scope.searcherName + '|' + 'Content', // (this whole tree path patttern needs to be refactored)
            'cultures-' + $scope.searcherName
        ]);

        //// view data
        //apiService.getViewDataForCultures($scope.searcherName)
        //    .then(function (response) { $scope.viewData = response.data; });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getCultureMatches($scope.searcherName, undefined, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();