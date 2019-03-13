(function () {
    'use strict';

    // service used to pass selected match from matches to the details controller
    angular
        .module('umbraco')
        .factory('Look.BackOffice.MatchService', MatchService);

    MatchService.$inject = [];

    function MatchService() {

        var selectedMatch = null;

        return {
            selectedMatch: selectedMatch
        };
    }

})();