(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagGroupController', TagGroupController);

    TagGroupController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService'];

    function TagGroupController($scope, $routeParams, apiService, $q, navigationService) {

        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];

        // sync tree
        navigationService.syncTree({
            tree: 'lookTree',
            path: [
                '-1',
                'searcher-' + $scope.searcherName,
                'tags-' + $scope.searcherName,
                'tagGroup-' + $scope.searcherName + '|' + $scope.tagGroup
            ],
            forceReload: false
        });

        // view data
        apiService
            .getViewDataForTagGroup($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.viewData = response.data;

                //$scope.tags = response.data.TagCounts;
            });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, $scope.tagGroup, undefined, sort, skip, take)
                .then(function (response) {

                    // TODO: foreach match, sort its tags collection, so that all tags in this group are first in the list (but keep them in existing order)

                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();