(function () {
    'use strict';

    // matches controller handles the paging and rendering of collection of matches (match data taken from parent scope)
    angular
        .module('umbraco')
        .controller('Look.BackOffice.MatchesController', MatchesController);

    MatchesController.$inject = ['$scope', 'Look.BackOffice.SortService', '$q', 'dialogService', 'Look.BackOffice.MatchService'];

    // this controller will handle paging for more results
    function MatchesController($scope, sortService, $q, dialogService, matchService) {

        dialogService.closeAll(); // close all on load

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

        // (later version of Angular adds one time binding)
        // returns true, if sorting by name, and the current match and last match differ in their initial name letter
        $scope.nameBreaker = function (currentMatch, lastMatch) {

            var enableNameBreaker = false;

            if (sortService.sortOn === 'Name') {
                if (angular.isUndefined(lastMatch)) { // first match
                    enableNameBreaker = true;
                } else {
                    enableNameBreaker = currentMatch.name.charAt(0).toLowerCase() !== lastMatch.name.charAt(0).toLowerCase();
                }
            }

            return enableNameBreaker;
        };

        $scope.dateBreaker = function (currentMatch, lastMatch) {

            var enableDateBreaker = false;

            if (sortService.sortOn === 'Date') {
                if (angular.isUndefined(lastMatch)) { // first match
                    enableDateBreaker = true;
                } else {

                    var currentDate = new Date(currentMatch.date);
                    var currentDay = currentDate.getDate() + currentDate.getMonth() + currentDate.getFullYear();

                    var lastDate = new Date(lastMatch.date);
                    var lastDay = lastDate.getDate() + lastDate.getMonth() + lastDate.getFullYear();

                    enableDateBreaker = currentDay !== lastDay;
                }
            }

            return enableDateBreaker;
        };

        $scope.showDetails = function (match) {
            
            matchService.selectedMatch = match;

            dialogService.open({
                template: '/App_Plugins/Look/BackOffice/LookTree/Partials/Details.html',
                show: true
            });
      };

      $scope.isActive = function (match) {
        return match === matchService.selectedMatch;
      };


        $scope.reload = function () {
            dialogService.closeAll();
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