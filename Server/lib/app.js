(function () {
    "use strict";
    var app = angular.module("codeSozluk", []);
    app.constant("apiConfig", {
        "baseUrl": "/v1/eksifeed/",
        "debe": "debe",
        "popular": "popular"
    });
    app.controller("home", function ($scope, $http, $window, apiConfig) {
        $scope.mm = "Working!";
        $scope.entries = [];
        $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
            $scope.entries = result;
        });
        $scope.redirect = function (link) {
            $window.open("http://www.eksisozluk.com/" + link, "_blank");
        };

    });
})(angular);