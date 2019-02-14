(function () {
    'use strict';

    // matches controller handles the paging and rendering of collection of matches (from parent scope)
    angular
        .module('umbraco')
        .controller('Look.BackOffice.MatchesController', MatchesController);

    MatchesController.$inject = ['$scope'];

    // this controller will handle paging for more results
    function MatchesController($scope) {

        var getMatches = $scope.$parent.getMatches; // $scope.$parent.getMatches(skip, take) expected to exist

        getMatches(0, 0) // skip 0, take all
            .then(function (matches) {
                $scope.matches = matches;
            });

    }

})();