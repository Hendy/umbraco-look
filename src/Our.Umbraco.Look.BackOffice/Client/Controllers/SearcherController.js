(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService'];

    function SearcherController($scope, $routeParams, apiService) {

        $scope.searcherName = $routeParams.id;
        
        apiService.getSearcherDetails($scope.searcherName)
            .then(function (response) {


        });


    }

})();