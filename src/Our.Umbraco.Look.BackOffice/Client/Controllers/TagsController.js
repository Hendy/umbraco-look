(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagsController', TagsController);

    TagsController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService'];

    function TagsController($scope, $routeParams, apiService, $q, navigationService) {

        // input params
        $scope.searcherName = $routeParams.id;

        // sync tree
        navigationService.syncTree({
            tree: 'lookTree',
            path: [
                '-1',
                'searcher-' + $scope.searcherName,
                'tags-' + $scope.searcherName
            ],
            forceReload: false
        });

        // view data
        apiService.getViewDataForTags($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;
            });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, undefined, undefined, sort, skip, take)
                .then(function (response) {
                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

    }

})();