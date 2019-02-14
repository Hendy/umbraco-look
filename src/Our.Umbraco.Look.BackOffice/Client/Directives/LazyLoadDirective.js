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

                var enabled = true;
                var expanding = false; // locker
                var previousScrollTop = 0;
                var scrollableElement = $(element).closest('.umb-panel-body'); // iQuery to find element that's scrollable

                scrollableElement.bind('scroll', function () {

                    // calculate direction of scroll
                    var currentScrollTop = $(this).scrollTop();
                    if (currentScrollTop > previousScrollTop) { // user scrolling down
                        if (!expanding && elementCanExpand()) {
                            lazyLoad();
                        }
                    }

                    previousScrollTop = currentScrollTop;
                });

                // disable directive if page changed
                scope.$watch(function () { return $location.url(); }, function (newValue, oldValue) {

                    if (newValue !== oldValue) {
                        console.log('disabling lazyLoad()');
                        enabled = false;
                        scrollableElement.unbind('scroll');
                    }
                });


                // HACK: add method to the scope so can be called from controller (we know this is the only instance of the directive being used)
                // the controller will call this after reductive filtering, or a complete clear
                scope.lazyLoad = function () {

                    $timeout(function () { // timeout to ensure scope is ready (and element height calculated correctly)

                        // TODO: cancel any lazy-load currenty in process
                        if (!expanding && elementCanExpand()) { // safety check;
                            lazyLoad();
                        }

                    });
                };

                lazyLoad(); // startup

                // --------------------------------------------------------------------------------

                // returns true if the element doesn't stretch below the bottom of the view
                function elementCanExpand() {
                    return element.offset().top + element.height() < $(window).height() + 100; // 100 = number of pixels below view
                }

                // handles the 'method to call', and attempts to fill the screen
                function lazyLoad() {

                    if (enabled) {

                        expanding = true;

                        $timeout(function () { // timeout to ensure scope is ready

                            scope.$apply(attrs.lazyLoad) // execute angular expression string (the 'method to call')
                                .then(function (tryAgain) { // return value of the promise

                                    if (tryAgain && elementCanExpand()) { // check to see if screen filled
                                        lazyLoad(); // try again
                                    }

                                    expanding = false;
                                });
                        });

                    }
                }
            }
        };
    }

})();