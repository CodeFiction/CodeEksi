(function (angular, $) {
    //var pattern = /\((bkz: )[A-Z a-z]+(\))/gi; ## regex for bkz.
    "use strict";
    angular.module("codeSozluk", ["ngRoute", "ngSanitize", "angular-loading-bar", "ngAnimate", "infinite-scroll"])
        .constant("apiConfig", {
            "baseUrl": "/v1/eksifeed/",
            "debe": "debe",
            "popular": "popular",
            "detail": "entries",
            "entries": "entries",
            "search": "titles/search",
            "titles": "titles"
        })
        .config(["$routeProvider", "cfpLoadingBarProvider", function ($routeProvider, cfpLoadingBarProvider) {
            $routeProvider.when("/", {
                templateUrl: "home/popular",
                controller: "homeController"
            })
            .when("/title/:title", {
                templateUrl: "home/detail",
                controller: "detailController"
            })
            .otherwise({
                redirectTo: "/"
            });
            cfpLoadingBarProvider.includeSpinner = false;
        }])
        .directive('ngSearch', function () {
            return function (scope, element) {
                $(element).blur(function (event) {
                    if (event.target.value.length > 0) {
                        return;
                    }
                    event.target.style.width = event.target.placeholder.length * 9 + 'px';
                });
                element.bind("keydown keypress", function (event) {
                    var e = event.target, factor = 1;
                    if (event.which === 8 || event.which === 46) {
                        factor = -1;
                    }
                    e.style.width = (e.value.length + factor) * 9 + 'px';

                    if (event.which === 13) {
                        scope.$apply(function () {
                            window.location.hash = "#/title/" + event.target.value;
                        });
                        event.preventDefault();
                    }
                });
            };
        })
        .controller("detailController", function ($rootScope, $scope, $http, $routeParams, apiConfig) {
            window.scrollTo(0, 0);
            $scope.title = {
                title: $routeParams.title
            };
            var bindToScope = function (result) {
                $scope.title.entries = result.entry_detail_models;
                $scope.title.id = result.title_name_id_text;
                $scope.currentPage = $scope.title.currentPage = parseInt(result.current_page);
                $scope.count = $scope.title.count = parseInt(result.page_count);
            };

            var generatePagedUrl = function (title, page) {
                return apiConfig.baseUrl + apiConfig.titles + "/" + $scope.title.id + "?page=" + page;
            }

            $scope.loadPaging = function () {
                var next = $scope.currentPage + 1;
                if (next < $scope.count + 1) {
                    $scope.currentPage = next;

                    $http.get(generatePagedUrl($scope.title.id, next)).success(function (result) {
                        Array.prototype.push.apply($scope.title.entries, result.entry_detail_models);
                        setTimeout(window.refreshNumberOfline, 300);
                    });
                }
            };
            $http.get(apiConfig.baseUrl + apiConfig.search + "/?titleText=" + $routeParams.title).success(function (result) {
                bindToScope(result);
            }).error(function (result, code) {
                if (code === 404 && result.length > 0) {
                    $scope.errorModels = result;
                }
            });
        })
        .controller("homeController", function ($scope, $http, $window, apiConfig) {
            $scope.entries = [];
            $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
                $scope.entries = result.debe_title_models;
                setTimeout(window.refreshNumberOfline, 100);
            });
        });
})(angular, $);

(function ($) {
    function calcNumberOfLine() {
        var fontHeight = parseInt($("body").css('line-height'));
        return parseInt($("body").height() / fontHeight)-20;
    };
    function appendLine(number) {
        var $li = $("<li>");
        $li.text(number);
        $("#sidebar ul").append($li);
    };
    function appendLineNumberItems() {
        var numberOfLine = calcNumberOfLine();
        if (numberOfLine <= $("#sidebar li").length) {
            return;
        }
        for (var i = 1; i <= numberOfLine - 2; i++) {
            appendLine(i);
        }
    };
    function removeNumberOfLine() {
        $("#sidebar ul").empty();
    }
    appendLineNumberItems();

    var refreshNumberOfline = window.refreshNumberOfline = function () {
        removeNumberOfLine();
        appendLineNumberItems();
    };


    $(document).ready(function () {
        refreshNumberOfline();
        $(window).resize(refreshNumberOfline);
    });
})(jQuery);