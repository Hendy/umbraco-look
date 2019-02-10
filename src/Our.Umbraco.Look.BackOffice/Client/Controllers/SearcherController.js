(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ViewDataService'];

    function SearcherController($scope, $routeParams, viewDataService) {

        $scope.searcherName = $routeParams.id;
        $scope.searcherDescription = null;
        $scope.searcherType = null;
        $scope.icon = null;
        
        viewDataService.getViewDataForSearcher($scope.searcherName)
            .then(function (response) {

                $scope.searcherDescription = response.data.SearcherDescription;
                $scope.searcherType = response.data.SearcherType;
                $scope.icon = response.data.Icon;


                $scope.response = response.data;

        });


    }

})();