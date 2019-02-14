(function () {
    'use strict';

    /*
        <element lazy-load="method to call"></element>

        this lazy load directive will keep calling the 'method to call' until the height of <element>
        fills the screen and while the method returns true (indicating that a retry could return more data)
    */
    angular
        .module('umbraco')
        .directive('lazyLoad', LazyLoadDirective);

    LazyLoadDirective.$inject = ['$timeout', '$location'];

    function LazyLoadDirective($timeout, $location) {

        return {
            restrict: 'A',
            link: function (scope, element, attrs) {

                var enabled = true; // dead switch
                var expanding = false; // locker
                var previousScrollTop = 0;
                var scrollableElement = $(element).closest('.umb-panel-body'); // jQuery to find element that's scrollable

                scrollableElement.bind('scroll', function () {
                    // calculate direction of scroll
                    var currentScrollTop = $(this).scrollTop();
                    if (currentScrollTop > previousScrollTop) { // user scrolling down
                        fireLazyLoad();
                    }

                    previousScrollTop = currentScrollTop;
                });

                // cleanup - disable directive if page changed
                scope.$watch(function () { return $location.url(); }, function (newValue, oldValue) {
                    if (newValue !== oldValue) {
                        enabled = false;
                        scrollableElement.unbind('scroll');
                    }
                });

                // add trigger method to scope, so controller can call this (eg. after reductive filtering, or a clear)
                scope.lazyLoad = function () {
                    //$timeout(function () { // timeout to ensure scope is ready (and element height calculated correctly)
                        fireLazyLoad();
                    //});
                };

                fireLazyLoad(); // startup

                // --------------------------------------------------------------------------------

                // returns true if the element doesn't stretch below the bottom of the view
                function elementCanExpand() {
                    return element.offset().top + element.height() < $(window).height() + 100; // 100 = number of pixels below view
                }

                var fireLazyLoadPromise = null;

                // private helper - common checks before will cause a lazyLoad to start
                function fireLazyLoad() {

                    $timeout.cancel(fireLazyLoadPromise);

                    if (enabled && !expanding && elementCanExpand()) { // check locks

                        // set trigger
                        fireLazyLoadPromise = $timeout(function () {
                            if (enabled && !expanding && elementCanExpand()) { // double check locks
                                expanding = true;
                                doLazyLoad();
                            }
                        }, 500);
                    }
                }

                // private helper - reccursive method (it holds the expanding flag)
                // handles the 'method to call', and attempts to fill the screen
                function doLazyLoad() {

                    //if (enabled) { // fail safe
                        //expanding = true; // ensures lock flag

                        $timeout(function () { // timeout to ensure scope is ready

                            scope.$apply(attrs.lazyLoad) // execute angular expression string (the 'method to call')

                                .then(function (tryAgain) { // return value of the promise, bool flag to indicate if the 'method to call' is worth repeating

                                    if (enabled && tryAgain && elementCanExpand()) {
                                        doLazyLoad(); // reccurse
                                    }

                                    expanding = false; // release lock flag
                                });
                        });

                    //}
                }
            }
        };
    }

})();