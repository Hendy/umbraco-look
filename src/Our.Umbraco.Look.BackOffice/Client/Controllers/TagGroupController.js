(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagGroupController', TagGroupController);

    TagGroupController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q'];

    function TagGroupController($scope, $routeParams, apiService, $q) {

        // input params
        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];

        // view data
        apiService
            .getViewDataForTagGroup($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.viewData = response.data;

                //$scope.tags = response.data.TagCounts;
            });

        // matches
        $scope.getMatches = function (skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, $scope.tagGroup)
                .then(function (response) {
                    q.resolve(response.data.Matches);
                });

            return q.promise;
        };
    }

})();