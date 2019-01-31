(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService'];

    function SearcherController($scope, $routeParams, apiService) {

        $scope.searcherName = $routeParams.id;
        $scope.searcherDescription = null;
        $scope.searcherType = null;
        
        apiService.getSearcherDetails($scope.searcherName)
            .then(function (response) {

                $scope.searcherDescription = response.data.SearcherDescription;
                $scope.searcherType = response.data.SearcherType;

                $scope.response = response;

        });


    }

})();