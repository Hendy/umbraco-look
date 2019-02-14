(function () {
    'use strict';

    // service used to pass sort data  between controllers
    angular
        .module('umbraco')
        .factory('Look.BackOffice.SortService', SortService);

    //SortService.$inject = [];

    function SortService() {

        return {
            sortOn : 'Score'
        };

    }

})();