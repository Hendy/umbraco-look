(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.DetailsController', DetailsController);

    DetailsController.$inject = ['$scope', 'dialogService', 'Look.BackOffice.MatchService'];

    function DetailsController($scope, dialogService, matchService) {

        $scope.viewData = {
            match: matchService.selectedMatch
        };

        $scope.hideDetails = function () {
            dialogService.closeAll();
            matchService.selectedMatch = null;
        };

    }

})();