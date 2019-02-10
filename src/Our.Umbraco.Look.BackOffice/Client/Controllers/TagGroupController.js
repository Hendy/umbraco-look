(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagGroupController', TagGroupController);

    TagGroupController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService'];

    function TagGroupController($scope, $routeParams, apiService) {

        var parsedId = $routeParams.id.split('|');

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.matches = null;

        //$scope.tags = null; // an object array of: tag name + useage count 

        apiService
            .getViewDataForTagGroup($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.response = response.data; // DEBUG

                //$scope.tags = response.data.TagCounts;
            });


        apiService
            .getMatches($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.matches = response.data;

            });


    }

})();