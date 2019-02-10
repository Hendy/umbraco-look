(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.ViewDataService', ViewDataService);

    ViewDataService.$inject = ['$http'];

    function ViewDataService($http) {

        return {
            getViewDataForSearcher: getViewDataForSearcher,
            getViewDataForTags: getViewDataForTags,
            getViewDataForTagGroup: getViewDataForTagGroup,
            getViewDataForTag: getViewDataForTag
        };
     
        function getViewDataForSearcher(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForSearcher',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        function getViewDataForTags(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForTags',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        function getViewDataForTagGroup(searcherName, tagGroup) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForTagGroup',
                params: {
                    'searcherName': searcherName,
                    'tagGroup': tagGroup
                }
            });
        }

        function getViewDataForTag(searcherName, tagGroup, tagName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForTag',
                params: {
                    'searcherName': searcherName,
                    'tagGroup': tagGroup,
                    'tagName': tagName
                }
            });
        }
    }

})();