(function () {
    'use strict';

    // service where each method corresponds to a C# API controller method
    angular
        .module('umbraco')
        .factory('Look.BackOffice.ApiService', ApiService);

    ApiService.$inject = ['$http'];

    function ApiService($http) {

        return {
            // get viewData
            getViewDataForSearcher: getViewDataForSearcher,
            getViewDataForRebuild: getViewDataForRebuild,
            getViewDataForNodes: getViewDataForNodes,
            getViewDataForNodeType: getViewDataForNodeType,
            getViewDataForCultures: getViewDataForCultures,
            getViewDataForCulture: getViewDataForCulture,
            getViewDataForTags: getViewDataForTags,
            getViewDataForTagGroup: getViewDataForTagGroup,
            getViewDataForTag: getViewDataForTag,
            getViewDataForLocations: getViewDataForLocations,

            // get matches
            getMatches: getMatches,
            getNodeTypeMatches: getNodeTypeMatches,
            getCultureMatches: getCultureMatches,
            getTagMatches: getTagMatches,
            getLocationMatches: getLocationMatches,

            // menu actions
            rebuildIndex: rebuildIndex,

            getConfigurationData: getConfigurationData // get details about indexers in use (sort will use to enable options)
        };

        // get viewData ----------------------------------------

        function getViewDataForSearcher(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForSearcher',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        function getViewDataForRebuild(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForRebuild',
                params: {
                    'searcherName': searcherName
                }
            });

        }
        function getViewDataForNodes(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForNodes',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        function getViewDataForNodeType(searcherName, nodeType) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForNodeType',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType
                }
            });
        }

        function getViewDataForCultures(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForCultures',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        function getViewDataForCulture(searcherName, lcid) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForCultures',
                params: {
                    'searcherName': searcherName,
                    'lcid': lcid
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

        function getViewDataForLocations(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForLocations',
                params: {
                    'searcherName': searcherName
                }
            });
        }

        // get matches ----------------------------------------

        function getMatches(searcherName, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetMatches',
                params: {
                    'searcherName': searcherName,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getNodeTypeMatches(searcherName, nodeType, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(nodeType)) { { nodeType = ''; }}
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetNodeTypeMatches',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getCultureMatches(searcherName, lcid, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(lcid)) { { lcid = -1; } }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetCultureMatches',
                params: {
                    'searcherName': searcherName,
                    'lcid': lcid,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getTagMatches(searcherName, tagGroup, tagName, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(tagGroup)) { tagGroup = ''; }
            if (angular.isUndefined(tagName)) { tagName = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetTagMatches',
                params: {
                    'searcherName': searcherName,
                    'tagGroup': tagGroup,
                    'tagName': tagName,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getLocationMatches(searcherName, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetLocationMatches',
                params: {
                    'searcherName': searcherName,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        // menu actions ----------------------------------------

        function rebuildIndex(indexerName) {
            return $http({
                method: 'POST',
                url: 'BackOffice/Look/Api/RebuildIndex',
                params: {
                    'indexerName': indexerName
                }
            });
        }

        function getConfigurationData(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetConfigurationData',
                params: {
                    'searcherName': searcherName
                }
            });
        }
    }

})();