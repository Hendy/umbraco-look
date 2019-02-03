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
        $scope.icon = null;
        
        apiService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) {

                $scope.searcherDescription = response.data.SearcherDescription;
                $scope.searcherType = response.data.SearcherType;
                $scope.icon = response.data.Icon;


                $scope.response = response.data;

        });


    }

})();