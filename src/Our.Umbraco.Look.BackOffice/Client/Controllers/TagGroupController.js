(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.TagGroupController', TagGroupController);

    TagGroupController.$inject = ['$scope', '$routeParams', 'Look.BackOffice.ApiService'];

    function TagGroupController($scope, $routeParams, apiService) {

        // init function used to get the tree node id from the view ! (as can't find a suitable resource to inject so as to get at this value)
        function init(currentNode) {
            //appenderName = currentNode.id.split('|')[1]; // strip the 'appender|' prefix        
        }
    }

})();