(function (angular) {
    "use strict";
    var app = angular.module("codeSozluk", [])
        .constant("apiConfig", {
            "baseUrl": "/v1/eksifeed/",
            "debe": "debe",
            "popular": "popular",
            "detail": "detail"
        })
        .controller("home", function ($scope, $http, $window, apiConfig) {
            $scope.mm = "Working!";
            $scope.entries = [];
            $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
                $scope.entries = result;
            });
            $scope.redirect = function (link) {
                $window.open("http://www.eksisozluk.com/" + link, "_blank");
            };
            $scope.openEntry = function (id, entry) {
                $scope.loading = true;
                $http.get(apiConfig.baseUrl + apiConfig.detail + "/" + id.substr(1)).success(function (result) {
                    entry.content = result;
                    $scope.loading = false; //poor man's loading indicator...
                });
            }
        });
})(angular);