(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.MatchesController', MatchesController);

    MatchesController.$inject = ['$scope', 'Look.BackOffice.ApiService'];

    // this controller will handle paging for more results
    function MatchesController($scope, apiService) {

        var searcherName = $scope.$parent.searcherName;
        var tagGroup = $scope.$parent.tagGroup;
        var tagName = $scope.$parent.tagName;

        apiService
            .getTagMatches(searcherName, tagGroup, tagName)
            .then(function (response) {

                $scope.matches = response.data;

            });

    }

})();