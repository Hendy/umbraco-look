(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagController', TagController);

    TagController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ViewDataService'];

    function TagController($scope, $routeParams, viewDataService) {

        var parsedId = $routeParams.id.split('|'); // limit to three, as tag may contain pipes (TODO: need to handle pipes)

        $scope.searcherName = parsedId[0];
        $scope.tagGroup = parsedId[1];
        $scope.tagName = parsedId[2];

        viewDataService.getViewDataForTag($scope.searcherName, $scope.tagGroup, $scope.tagName)
            .then(function (response) {

                $scope.response = response.data;

            });

    }

})();