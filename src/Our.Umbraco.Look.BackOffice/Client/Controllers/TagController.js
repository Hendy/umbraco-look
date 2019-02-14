(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagController', TagController);

    TagController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q'];

    function TagController($scope, $routeParams, apiService, $q) {

        // input params
        var parsedId = $routeParams.id.split('|'); // limit to three, as tag may contain pipes (TODO: need to handle pipes)

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.tagName = parsedId[2];

        // view data
        apiService
            .getViewDataForTag($scope.searcherName, $scope.tagGroup, $scope.tagName)
            .then(function (response) {

                $scope.viewData = response.data;

            });

        // matches
        $scope.getMatches = function (skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName, $scope.tagGroup, $scope.tagName)
                .then(function (response) {
                    q.resolve(response.data.Matches);
                });

            return q.promise;
        };

    }

})();