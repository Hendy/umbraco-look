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

        apiService.getViewDataForTagGroup($scope.searcherName, $scope.tagGroup)
            .then(function (response) {

                $scope.response = response;

            });

    }

})();