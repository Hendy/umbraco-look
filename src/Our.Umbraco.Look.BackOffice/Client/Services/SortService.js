(function () {
    'use strict';

    // service used to pass sort data  between controllers
    angular
        .module('umbraco')
        .factory('Look.BackOffice.SortService', SortService);

    SortService.$inject = [];

    function SortService() {

        var sortOn = 'Score';
        var callbacks = [];

        return {
            // when a change is executed
            change: function (sort) { 

                this.sortOn = sort;

                angular.forEach(callbacks, function (callback) {
                    callback();
                });
            },

            // the current sort on property
            sortOn: sortOn,

            // register a callback to be triggered on change
            onChange: function (callback) {
                callbacks.push(callback);
            }
        };

    }

})();