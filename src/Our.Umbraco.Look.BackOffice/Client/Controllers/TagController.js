(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagController', TagController);

    TagController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ViewDataService', 'Look.BackOffice.QueryService'];

    function TagController($scope, $routeParams, viewDataService, queryService) {

        var parsedId = $routeParams.id.split('|'); // limit to three, as tag may contain pipes (TODO: need to handle pipes)

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.tagName = parsedId[2];
        $scope.matches = null;

        viewDataService.getViewDataForTag($scope.searcherName, $scope.tagGroup, $scope.tagName)
            .then(function (response) {

                $scope.response = response.data;

            });


        queryService
            .getMatches($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.matches = response.data.matches;

            });

    }

})();