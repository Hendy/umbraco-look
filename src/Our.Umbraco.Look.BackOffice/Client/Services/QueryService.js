(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.QueryService', QueryService);

    QueryService.$inject = ['$http', '$q'];

    function QueryService($http, $q) {

        return {
            getMatches: getMatches
        };



        function getMatches(searcherName, tagGroup, tagName, sort, skip, take) {

            var deferred = $q.defer();

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(tagGroup)) { tagGroup = ''; }
            if (angular.isUndefined(tagName)) { tagName = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = ''; }
            if (angular.isUndefined(take)) { take = ''; }

            var matches = $http({
                                method: 'GET',
                                url: 'BackOffice/Look/Api/Query',
                                params: {
                                    'searcherName': searcherName,
                                    'tagGroup': tagGroup,
                                    'tagName': tagName,
                                    'sort': sort,
                                    'skip': skip,
                                    'take': take
                                }
            });

            
            deferred.resolve(matches);

            return deferred.promise;
        }
    }

})();