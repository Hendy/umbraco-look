(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.RebuildController', RebuildController);

    RebuildController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService', 'navigationService'];

    function RebuildController($scope, $routeParams, apiService, navigationService) {

        //console.log($routeParams);
       /// console.log($scope.currentNode);

        //var searcherName;

        //// init function used to get the tree node id from the view ! (as can't find a suitable resource to inject so as to get at this value)
        //// the current node represents the tree node associated with the current menu option
        $scope.init = function (currentNode) {

            console.log(currentNode);

            $scope.searcherName = currentNode.id.split('-')[1]; // strip the 'appender|' prefix

            apiService.getViewDataForRebuild($scope.searcherName)
                .then(function (response) { $scope.viewData = response.data; });

        };


        $scope.hide = function () {
            navigationService.hideNavigation();
        };

        $scope.rebuild = function () {

        };
    }

})();