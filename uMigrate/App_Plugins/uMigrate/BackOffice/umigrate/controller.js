angular.module("umbraco").controller('mig.ViewController', ['$scope', '$routeParams', '$http', 'umbRequestHelper', 'notificationsService', 'assetsService', function($scope, $routeParams, $http, umbRequestHelper, notificationsService, assetsService) {
    'use strict';
    assetsService.loadCss('/App_Plugins/uMigrate/BackOffice/umigrate/edit.css');

    var url = umbRequestHelper.getApiUrl("uMigrate.MigrationTreeController", "GetTreeNodeData");
    $http.get(url, { params: { id: $routeParams.id } })
         .success(function(data) { $scope.model = data; })
         .error(function(e) { notificationsService.error("Error", e.ExceptionMessage || e.Message || e); });
}]);