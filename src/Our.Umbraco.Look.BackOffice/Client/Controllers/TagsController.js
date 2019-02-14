(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagsController', TagsController);

    TagsController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', '$q'];

    function TagsController($scope, $routeParams, apiService, $q) {

        // input params
        $scope.searcherName = $routeParams.id;

        // view data
        apiService.getViewDataForTags($scope.searcherName)
            .then(function (response) {
                $scope.viewData = response.data;
            });

        // matches
        $scope.getMatches = function (sort, skip, take) {

            var q = $q.defer();

            apiService
                .getTagMatches($scope.searcherName)
                .then(function (response) {
                    q.resolve(response.data.Matches);
                });

            return q.promise;
        };

    }

})();