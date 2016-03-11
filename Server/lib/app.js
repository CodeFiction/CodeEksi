(function (angular) {
    //var pattern = /\((bkz: )[A-Z a-z]+(\))/gi; ## regex for bkz.
    "use strict";
    angular.module("codeSozluk", ["ngRoute", "ngSanitize","angular-loading-bar", "ngAnimate"])
        .constant("apiConfig", {
            "baseUrl": "/v1/eksifeed/",
            "debe": "debe",
            "popular": "popular",
            "detail": "entries",
            "entries": "entries",
            "search": "titles/search"
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
        .controller("detailController", function ($scope, $http, $routeParams, apiConfig) {
            $scope.title = {
                title: $routeParams.title
            };
            $http.get(apiConfig.baseUrl + apiConfig.search + "/?titleText=" + $routeParams.title).success(function (result) {
                $scope.title.entries = result.entry_detail_models;
            });
        })
        .controller("homeController", function ($scope, $http, $window, apiConfig) {
            $scope.entries = [];
            $scope.openEntry = function (id, entry) {
                $http.get(apiConfig.baseUrl + apiConfig.entries + "/" + id.substr(1)).success(function (result) {
                    entry.content = result;
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