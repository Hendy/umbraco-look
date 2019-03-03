(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagGroupController', TagGroupController);

    TagGroupController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function TagGroupController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'tags-' + $scope.searcherName,
            'tagGroup-' + $scope.searcherName + '|' + $scope.tagGroup
        ]);

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

                    // remove link to this tag group from all matches (no need to render link to self)
                    response.data.matches = response.data.matches.map(function (item) {

                        item.tagGroups = item.tagGroups.map(function (tagGroup) {
                            if (tagGroup.name === $scope.tagGroup) {
                                tagGroup.link = null; // disable link to self
                            } 

                            return tagGroup;
                        });

                        return item;
                    });

                    // TODO: sort so that this tag group is first in the list

                    q.resolve(response.data.matches);
                });

            return q.promise;
        };
    }

})();