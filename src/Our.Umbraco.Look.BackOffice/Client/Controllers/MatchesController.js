(function () {
    'use strict';

    // matches controller handles the paging and rendering of collection of matches (from parent scope)
    angular
        .module('umbraco')
        .controller('Look.BackOffice.MatchesController', MatchesController);

    MatchesController.$inject = ['$scope', 'Look.BackOffice.SortService', '$q'];

    // this controller will handle paging for more results
    function MatchesController($scope, sortService, $q) {

        $scope.matches = []; // full collection of matches to render
        $scope.currentlyLoading = false;
        $scope.finishedLoading = false;

        var getMatches = $scope.$parent.getMatches; // $scope.$parent.getMatches(sort, skip, take) expected to exist

        var skip = 0; // skip counter
        const take = 1; // take 



        $scope.getMoreMatches = function () {

            var q = $q.defer();

            if (!$scope.currentlyLoading) {

                $scope.currentlyLoading = true;

                getMatches(sortService.sortOn, skip, take)
                    .then(function (matches) { // success
                        
                        var tryAgain = false;

                        if (matches.length > 0) {
                            $scope.matches = $scope.matches.concat(matches);
                            tryAgain = true;
                            skip += take;
                        }

                        $scope.currentlyLoading = false;
                        q.resolve(tryAgain);

                    });
            } else {
                q.resolve(true); // we're currently loading data, but return true to indicate consumer should try again
            }

            return q.promise;
        };


        sortService.onChange(function () {
            $scope.matches = [];
            skip = 0;

            //$scope.lazyLoad(); // trigger the lazy load
        });

    }

})();