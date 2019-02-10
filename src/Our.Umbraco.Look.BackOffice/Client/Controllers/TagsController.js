(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagsController', TagsController);

    TagsController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ViewDataService'];

    function TagsController($scope, $routeParams, viewDataService) {

        $scope.searcherName = $routeParams.id;

        viewDataService.getViewDataForTags($scope.searcherName)
            .then(function (response) {

                $scope.response = response.data;

            });
    }

})();