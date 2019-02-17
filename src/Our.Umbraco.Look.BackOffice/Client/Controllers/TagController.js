﻿(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagController', TagController);

    TagController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q', 'navigationService'];

    function TagController($scope, $routeParams, apiService, $q, navigationService) {

        // input params
        var parsedId = $routeParams.id.split('|'); // limit to three, as tag may contain pipes (TODO: need to handle pipes)

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.tagName = parsedId[2];

        // sync tree
        navigationService.syncTree({
            tree: 'lookTree',
            path: [
                '-1',
                'searcher-' + $scope.searcherName,
                'tags-' + $scope.searcherName,
                'tagGroup-' + $scope.searcherName + '|' + $scope.tagGroup,
                'tag-' + $scope.searcherName + '|' + $scope.tagGroup + '|' + $scope.tagName
            ],
            forceReload: false
        });

        // view data
        apiService
            .getViewDataForTag($scope.searcherName, $scope.tagGroup, $scope.tagName)
            .then(function (response) {

                $scope.viewData = response.data;

            });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, $scope.tagGroup, $scope.tagName, sort, skip, take)
                .then(function (response) {

                    // TODO: foreach match, sort its tags collection, so that this tag is first in the list (followed by all other tags in this group)

                    q.resolve(response.data.matches);
                });

            return q.promise;
        };

    }

})();