﻿(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagController', TagController);

    TagController.$inject = ['$scope', '$routeParams', '$q', 'Look.BackOffice.ApiService', 'Look.BackOffice.TreeService'];

    function TagController($scope, $routeParams, $q, apiService, treeService) {

        // input params
        var parsedId = $routeParams.id.split('|'); // limit to three, as tag may contain pipes (TODO: need to handle pipes)

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.tagName = parsedId[2];

        treeService.update([
            '-1',
            'searcher-' + $scope.searcherName,
            'tags-' + $scope.searcherName,
            'tagGroup-' + $scope.searcherName + '|' + $scope.tagGroup,
            'tag-' + $scope.searcherName + '|' + $scope.tagGroup + '|' + $scope.tagName
        ]);

        // view data
        apiService
            .getViewDataForTag($scope.searcherName, $scope.tagGroup, $scope.tagName)
            .then(function (response) { $scope.viewData = response.data; });

        // filters
        $scope.getFilters = function () {

            var q = $q.defer();

            apiService
                .getTagFilters($scope.searcherName, $scope.tagGroup, $scope.tagName)
                .then(function (response) {
                    q.resolve(response.data);
                });

            return q.promise;
        };

        // matches
        $scope.getMatches = function (filter, sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, $scope.tagGroup, $scope.tagName, filter, sort, skip, take)
                .then(function (response) {

                    // remove link to this tag from all matches (no need to render link to self)
                    response.data.matches = response.data.matches.map(function (item) {

                        item.tagGroups = item.tagGroups.map(function (tagGroup) {
                            tagGroup.tags = tagGroup.tags.map(function (tag) {

                                if (tagGroup.name === $scope.tagGroup && tag.name === $scope.tagName) {
                                    tag.link = null; // clear link to self
                                }

                                return tag;
                            });                                

                            return tagGroup;
                        });

                        return item;
                    });

                    // TODO: foreach match, sort its tags collection, so that this tag is first in the list (followed by all other tags in this group)

                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

    }

})();