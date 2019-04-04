(function () {
    'use strict';

    angular
        .module('umbraco')
        .factory('Look.BackOffice.FiltersService', FiltersService);

    FiltersService.$inject = [];

    function FiltersService() {

        // alias dropdown could be Content / Media / Member
        var callbacks = [];

        return {
            onChange: onChange
        };

        // register a callback to be triggered on change
        function onChange(callback) { callbacks.push(callback); }

        //// when a change occurs
        //function change(sort) {
        //    angular.forEach(callbacks, function (callback) { callback(); });
        //}

    }

})();