(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.CultureController', CultureController);

    CultureController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function CultureController($scope, $routeParams, $q, apiService, treeService)
{
        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.lcid = parsedId[1];

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'nodes-' + $scope.searcherName,
            'nodeType-' + $scope.searcherName + '|' + 'Content', // (this whole tree path patttern needs to be refactored)
            'culture-' + $scope.searcherName + '|' + $scope.lcid
        ]);

        // view data
        apiService.getViewDataForCulture($scope.searcherName, $scope.lcid)
            .then(function (response) { $scope.viewData = response.data; });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getCultureMatches($scope.searcherName, $scope.lcid, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

}

})();