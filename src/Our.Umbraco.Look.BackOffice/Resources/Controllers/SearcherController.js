(function () {
    'use strict';

    angular
        .module('umbraco')
        .controller('Look.BackOffice.SearcherController', SearcherController);

    SearcherController.$inject = ['$scope'];

    function SearcherController($scope) {

        $scope.test = 'nothing';

        // init function used to get the tree node id from the view ! (as can't find a suitable resource to inject so as to get at this value)
        function init(currentNode) {
            appenderName = currentNode.id.split('|')[1]; // strip the 'appender|' prefix
            load();
        }

        function load() {

            $scope.test = "loaded";

            //azureLoggerResource.getDetails(appenderName)
            //    .then(function (response) {

            //        $scope.appenderName = response.data.name;
            //        $scope.connectionString = response.data.connectionString;
            //        $scope.tableName = response.data.tableName;
            //        $scope.readOnly = response.data.readOnly;
            //        $scope.bufferSize = response.data.bufferSize;

            //    });
        }

        //function cancel() {
        //    navigationService.hideNavigation();
        //}
    }

})();