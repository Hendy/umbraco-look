(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.FiltersService', FiltersService);

    FiltersService.$inject = [];

    function FiltersService() {

        var callbacks = [];
        var filterAlias = null; // could be any docType, mediaType or memberType alias

        return {
            onChange: onChange,
            change: change,
            filterAlias: filterAlias
        };

        // register a callback to be triggered on change
        function onChange(callback) { callbacks.push(callback); }

        // when a change occurs
        function change(filterAlias) {
            this.filterAlias = filterAlias;
            angular.forEach(callbacks, function (callback) { callback(); });
        }
    }

})();