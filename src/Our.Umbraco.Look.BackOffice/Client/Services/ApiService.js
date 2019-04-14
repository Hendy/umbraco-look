(function () {
    'use strict';

    // flat service layer where each method corresponds to a C# API controller method
    angular
        .module('umbraco')
        .factory('Look.BackOffice.ApiService', ApiService);

    ApiService.$inject = ['$http'];

    function ApiService($http) {

        return {
            // get viewData
            getViewDataForSearcher: getViewDataForSearcher,
            getViewDataForRebuild: getViewDataForRebuild,
            getViewDataForNodeType: getViewDataForNodeType,
            getViewDataForDetached: getViewDataForDetached,
            getViewDataForTags: getViewDataForTags,
            getViewDataForTagGroup: getViewDataForTagGroup,
            getViewDataForTag: getViewDataForTag,
            getViewDataForLocations: getViewDataForLocations,

            // get filters
            getFilters: getFilters,
            getNodeTypeFilters: getNodeTypeFilters,
            getDetachedFilters: getDetachedFilters,
            getTagFilters: getTagFilters,
            getLocationFilters: getLocationFilters,

            // get matches
            getMatches: getMatches,
            getNodeTypeMatches: getNodeTypeMatches,
            getDetachedMatches: getDetachedMatches,
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
                params: { 'searcherName': searcherName }
            });
        }

        function getViewDataForRebuild(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForRebuild',
                params: { 'searcherName': searcherName }
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

        function getViewDataForDetached(searcherName, nodeType) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForDetached',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType
                }
            });
        }

        function getViewDataForTags(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetViewDataForTags',
                params: { 'searcherName': searcherName }
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
                params: { 'searcherName': searcherName }
            });
        }

        // get filters ----------------------------------------

        function getFilters(searcherName) {
            if (angular.isUndefined(searcherName)) { searcherName = ''; }

            var filters = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetFilters',
                params: { 'searcherName': searcherName }
            });

            return filters;
        }

        function getNodeTypeFilters(searcherName, nodeType) {
            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(nodeType)) { { nodeType = ''; } }

            var filters = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetNodeTypeFilters',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType
                }
            });

            return filters;
        }

        function getDetachedFilters(searcherName, nodeType) {
            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(nodeType)) { { nodeType = ''; } }

            var filters = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetDetachedFilters',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType
                }
            });

            return filters;
        }

        function getTagFilters(searcherName, tagGroup, tagName) {
            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(tagGroup)) { tagGroup = ''; }
            if (angular.isUndefined(tagName)) { tagName = ''; }

            var filters = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetTagFilters',
                params: {
                    'searcherName': searcherName,
                    'tagGroup': tagGroup,
                    'tagName': tagName
                }
            });

            return filters;
        }

        function getLocationFilters(searcherName) {
            if (angular.isUndefined(searcherName)) { searcherName = ''; }

            var filters = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetLocationFilters',
                params: { 'searcherName': searcherName }
            });

            return filters;
        }

        // get matches ----------------------------------------

        function getMatches(searcherName, filter, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(filter)) { filter = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetMatches',
                params: {
                    'searcherName': searcherName,
                    'filter': filter,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getNodeTypeMatches(searcherName, nodeType, filter, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(nodeType)) { { nodeType = ''; }}
            if (angular.isUndefined(filter)) { filter = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetNodeTypeMatches',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType,
                    'filter': filter,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }


        function getDetachedMatches(searcherName, nodeType, filter, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(nodeType)) { { nodeType = ''; } }
            if (angular.isUndefined(filter)) { filter = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetDetachedMatches',
                params: {
                    'searcherName': searcherName,
                    'nodeType': nodeType,
                    'filter': filter,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getTagMatches(searcherName, tagGroup, tagName, filter, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(tagGroup)) { tagGroup = ''; }
            if (angular.isUndefined(tagName)) { tagName = ''; }
            if (angular.isUndefined(filter)) { filter = ''; }
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
                    'filter': filter,
                    'sort': sort,
                    'skip': skip,
                    'take': take
                }
            });

            return matches;
        }

        function getLocationMatches(searcherName, filter, sort, skip, take) {

            if (angular.isUndefined(searcherName)) { searcherName = ''; }
            if (angular.isUndefined(filter)) { filter = ''; }
            if (angular.isUndefined(sort)) { sort = ''; }
            if (angular.isUndefined(skip)) { skip = 0; }
            if (angular.isUndefined(take)) { take = 0; }

            var matches = $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetLocationMatches',
                params: {
                    'searcherName': searcherName,
                    'filter': filter,
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
                params: { 'indexerName': indexerName }
            });
        }

        function getConfigurationData(searcherName) {
            return $http({
                method: 'GET',
                url: 'BackOffice/Look/Api/GetConfigurationData',
                params: { 'searcherName': searcherName }
            });
        }
    }

})();