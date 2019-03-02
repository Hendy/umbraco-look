(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.TreeService', TreeService);

    TreeService.$inject = ['navigationService', '$timeout'];

    function TreeService(navigationService, $timeout) {

        return { update: update };

        // register a callback to be triggered on change
        function update(path) {
            console.log('tree service !');
            $timeout(function () { // timeout required to ensure navigationService ready
                navigationService.syncTree({
                    tree: 'lookTree',
                    path: path,
                    forceReload: true
                });
            }, 500);

        }
    }

})();