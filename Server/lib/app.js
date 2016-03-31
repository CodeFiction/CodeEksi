(function (angular, $) {
    //var pattern = /\((bkz: )[A-Z a-z]+(\))/gi; ## regex for bkz.
    "use strict";
    angular.module("codeSozluk", ["ngRoute", "ngSanitize", "angular-loading-bar", "ngAnimate", "infinite-scroll"])
        .constant("apiConfig", {
            "baseUrl": "/v1/eksifeed/",
            "debe": "debe",
            "popular": "populer",
            "detail": "entries",
            "entries": "entries",
            "search": "titles/search",
            "titles": "titles"
        })
        .factory("storage", [function () {
            return (function () {
                return {
                    read: function (key) {
                        return localStorage.getItem(key);
                    },
                    write: function (key, value) {
                        return localStorage.setItem(key, value);
                    }
                };
            }());
        }])
        .config(["$routeProvider", "cfpLoadingBarProvider", function ($routeProvider, cfpLoadingBarProvider) {
            $routeProvider.when("/", {
                templateUrl: "home/mostLiked",
                controller: "homeController"
            })
            .when("/popular", {
                templateUrl: "home/popular",
                controller: "popularController"
            })
            .when("/title/:title", {
                templateUrl: "home/detail",
                controller: "detailController"
            })
            .when("/entry/:entryId", {
                templateUrl: "home/entry",
                controller: "entryController"
            })
            .when("/about", {
                templateUrl: "home/about",
                controller: "aboutController"
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
                    var e = event.target;
                    var factor = 1;
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
        .controller("detailController", ["$rootScope", "$scope", "$http", "$routeParams", "apiConfig", "storage", function ($rootScope, $scope, $http, $routeParams, apiConfig, storage) {
            window.scrollTo(0, 0);
            $rootScope.okay = storage.read("theme") === "true";
            $rootScope.$watch("okay", function (n, o) {
                storage.write("theme", n);
            });

            $scope.title = {
                title: $routeParams.title
            };
            var bindToScope = function (result) {
                $scope.title.entries = result.entry_detail_models;
                $scope.title.id = result.title_name_id_text;
                $scope.currentPage = parseInt(result.current_page);
                $scope.title.currentPage = $scope.currentPage;
                $scope.count = parseInt(result.page_count);
                $scope.title.count = $scope.count;
            };

            var generatePagedUrl = function (title, page) {
                return apiConfig.baseUrl + apiConfig.titles + "/" + $scope.title.id + "?page=" + page;
            };

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
        }])
        .controller('entryController', ["$scope", "$http", "$routeParams", "apiConfig", "storage", "$rootScope", function ($scope, $http, $routeParams, apiConfig, storage, $rootScope) {
            var entryId = $routeParams.entryId;
            window.scrollTo(0, 0);
            $rootScope.okay = storage.read("theme") === "true";
            $rootScope.$watch("okay", function (n, o) {
                storage.write("theme", n);
            });

            $scope.title = {
                title: ""
            };
            var bindToScope = function (result) {
                $scope.title.entries = [];
                $scope.title.entries.push(result);
            };

            $http.get(apiConfig.baseUrl + apiConfig.entries + "/" + entryId).success(function (result) {
                bindToScope(result);
                setTimeout(window.refreshNumberOfline, 100);
            }).error(function (result, code) {
                if (code === 404 && result.length > 0) {
                    $scope.errorModels = result;
                }
            });

        }])
        .controller('popularController', ["$scope", "$http", "$window", "apiConfig", "storage", "$rootScope", function ($scope, $http, $window, apiConfig, storage, $rootScope) {
            $rootScope.okay = storage.read("theme") === "true";
            $rootScope.$watch("okay", function (n, o) {
                storage.write("theme", n);
            });
            $scope.currentPage = 1;
            $scope.titles = [];
            $http.get(apiConfig.baseUrl + apiConfig.popular).success(function (result) {
                $scope.titles = result.populer_title_models;
                setTimeout(window.refreshNumberOfline, 100);
            });
            var generatePagedUrl = function (title, page) {
                return apiConfig.baseUrl + apiConfig.popular + "/" + "?page=" + page;
            };
            $scope.loadPaging = function () {
                var next = $scope.currentPage + 1;
                if (next < $scope.count + 1) {
                    $scope.currentPage = next;
                    $http.get(generatePagedUrl($scope.title.id, next)).success(function (result) {
                        Array.prototype.push.apply($scope.title, result.populer_title_models);
                        setTimeout(window.refreshNumberOfline, 300);
                    });
                }
            };
        }])
        .controller('aboutController', ["$scope", function ($scope) {

        }])
        .controller("homeController", ["$scope", "$http", "$window", "apiConfig", "storage", "$rootScope", function ($scope, $http, $window, apiConfig, storage, $rootScope) {
            $scope.entries = [];
            $rootScope.okay = storage.read("theme") === "true";
            $rootScope.$watch("okay", function (n, o) {
                storage.write("theme", n);
            });
            $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
                $scope.entries = result.debe_title_models;
                setTimeout(window.refreshNumberOfline, 100);
            });
        }]);
}(angular, $));

(function ($) {
    "use strict";
    function calcNumberOfLine() {
        var fontHeight = parseInt($("body").css('line-height'));
        return parseInt($("body").height() / fontHeight) - 20;
    }
    function appendLine(number) {
        var $li = $("<li>");
        $li.text(number);
        $("#sidebar ul").append($li);
    }
    function appendLineNumberItems() {
        var numberOfLine = calcNumberOfLine();
        if (numberOfLine <= $("#sidebar li").length) {
            return;
        }
        var idx;
        for (idx = 1; idx <= numberOfLine - 2; idx++) {
            appendLine(idx);
        }
    }
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
}(jQuery));