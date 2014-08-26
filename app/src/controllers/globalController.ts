module Application {

    var app = angular.module("app");

    app.controller("GlobalController", ["$scope", ($scope)
        => new GlobalControlller($scope)]);


    export interface IGlobalControllerScope extends ng.IScope {
        aboutActive: boolean;
        rnd: number;
        toggleAbout: any;
    }

    export class GlobalControlller {

        scope: IGlobalControllerScope;

        someText: string;

        constructor(
            $scope: IGlobalControllerScope
            ) {
            this.scope = $scope;

            $scope.aboutActive = false;

            $scope.rnd = Math.random();
            this.someText = "bla bla";
        }
    }
}