(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.DetailsController', DetailsController);

    DetailsController.$inject = ['$scope', 'Look.BackOffice.MatchService'];

    function DetailsController($scope, matchService) {

        $scope.viewData = {
            debug: matchService
        };

    }

})();