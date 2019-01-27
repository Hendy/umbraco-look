(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.ApiService', ApiService);

    ApiService.$inject = ['$rootScope', '$http', '$q'];

    function ApiService($rootScope, $http, $q) {

        return {
            getSearcherDetails: getSearcherDetails
        };
     
        function getSearcherDetails(searcherName) {

            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetSearcherDetails',
                params: {
                    'searcherName': searcherName
                }
            });
        }

    }

})();