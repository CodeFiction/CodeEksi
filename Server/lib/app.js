(function (angular) {
    //var pattern = /\((bkz: )[A-Z a-z]+(\))/gi; ## regex for bkz.
    "use strict";
    angular.module("codeSozluk", ["ngRoute", "ngSanitize", "angular-loading-bar", "ngAnimate"])
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
                element.bind("keydown keypress", function (event) {
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
            $scope.title = {
                title: $routeParams.title
            };
            var bindToScope = function (result) {
                $scope.title.entries = result.entry_detail_models;
                $scope.title.id = result.title_name_id_text;
                //there might be a binding problem here??
                $rootScope.currentPage = $scope.title.currentPage = parseInt(result.page_count);
                $rootScope.count = $scope.title.count = parseInt(result.current_page);
            };

            var generatePagedUrl = function (title, page) {
                return apiConfig.baseUrl + apiConfig.titles + "/" + $scope.title.id + "?page=" + page;
            }

            $rootScope.paging = true;
            $rootScope.previous = function () {
                var prev = $scope.currentPage = $scope.title.currentPage - 1;
                if (prev > 0) {
                    $http.get(generatePagedUrl($scope.title.id, prev)).success(function (result) {
                        bindToScope(result);
                    });
                }

            };
            $rootScope.next = function () {
                var next = $scope.currentPage = $scope.title.currentPage + 1;
                if (next < $scope.title.count + 1) {
                    $http.get(generatePagedUrl($scope.title.id, next)).success(function (result) {
                        bindToScope(result);
                    });
                }

            };
            $http.get(apiConfig.baseUrl + apiConfig.search + "/?titleText=" + $routeParams.title).success(function (result) {
                bindToScope(result);
            });
        })
        .controller("homeController", function ($scope, $http, $window, apiConfig) {
            $scope.entries = [];
            $scope.openEntry = function (id, entry) {
                $http.get(apiConfig.baseUrl + apiConfig.entries + "/" + id.substr(1)).success(function (result) {
                    entry.content = result;
                    //if (entry.content.length > 200) {
                    //    entry.shortContent = entry.content.substr(200);
                    //    entry.showMore = false;
                    //} else {
                    //    entry.showMore = true;
                    //}
                });
            };
            $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
                $scope.entries = result;
                for (var idx = 0; idx < $scope.entries.length; idx++) {
                    var $entry = $scope.entries[idx];
                    $scope.openEntry($entry.entry_id, $entry);
                }
            });
        });
})(angular);