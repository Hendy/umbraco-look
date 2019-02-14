(function () {
    'use strict';

    // service used to pass sort data  between controllers
    angular
        .module('umbraco')
        .factory('Look.BackOffice.SortService', SortService);

    SortService.$inject = [];

    function SortService() {

        var callbacks = [];
        var sortOn = 'Score'; // the current sort on property

        return {
            onChange: onChange,
            change: change,
            sortOn: sortOn
        };

        // register a callback to be triggered on change
        function onChange(callback) {
            callbacks.push(callback);
        }

        // when a change is executed
        function change(sort) {
            sortOn = sort;

            angular.forEach(callbacks, function (callback) {
                callback();
            });
        }
    }

})();