(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.QueryService', QueryService);

    QueryService.$inject = ['$http'];

    function QueryService($http) {

        return {
            getMatches: getMatches
        };



        function getMatches(searcherName, skip, take, tagGroup, tagName) {

        }
    }

})();