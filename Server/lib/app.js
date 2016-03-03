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
            $scope.openEntry = function (id, entry) {
                $http.get(apiConfig.baseUrl + apiConfig.detail + "/" + id.substr(1)).success(function (result) {
                    entry.content = result;
                });
            }
            $http.get(apiConfig.baseUrl + apiConfig.debe).success(function (result) {
                $scope.entries = result;
                for (var idx = 0; idx < $scope.entries.length; idx++) {
                    var $entry = $scope.entries[idx];
                    $scope.openEntry($entry.entry_id, $entry);
                }
            });
        });
})(angular);