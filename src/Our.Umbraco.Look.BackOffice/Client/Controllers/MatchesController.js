﻿(function () {
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
        const take = 15; // (set to 1 specifically for bad performance during development)

        // prepare method for lazyLoad to call
        $scope.getMoreMatches = function () {

            var q = $q.defer();

            if (!$scope.finishedLoading && !$scope.currentlyLoading) {
                $scope.currentlyLoading = true;

                getMatches(sortService.sortOn, skip, take)
                    .then(function (matches) { // success

                        var tryAgain = false;

                        if (matches.length > 0) {
                            $scope.matches = $scope.matches.concat(matches);
                            tryAgain = true;
                            skip += take;
                        }
                        else {
                            $scope.finishedLoading = true;
                        }

                        $scope.currentlyLoading = false;

                        q.resolve(tryAgain);
                    });

            } else {
                q.resolve(true); // we're currently loading data, but do try again (only finish when no more data is returned)
            }

            return q.promise;
        };

        $scope.cancelGetMoreMatches = function () {
            // TODO: 
        };

        $scope.refresh = function () {
            reset();
        };

        //$scope.lazyLoad(); // trigger the lazy load to start - wasn't ready here

        // clear data, then re-trigger lazy-load
        sortService.onChange(function () {
            reset();
        });

        function reset() {
            skip = 0;
            $scope.matches = [];
            $scope.finishedLoading = false;
            $scope.lazyLoad(); // trigger the lazy load
        }
    }

})();