/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("HomeController", ["$scope", "$interval", ($scope, $interval)
        => new HomeController($scope, $interval)]);


    export interface IHomeControllerScope extends ng.IScope {
        displayText: string;
        counter: number;
        aList: string[];
    }

    export class HomeController {
        scope: IHomeControllerScope;
        interval: ng.IIntervalService;

        constructor(
            $scope: IHomeControllerScope,
            $interval: ng.IIntervalService
            ) {
            this.scope = $scope;
            this.interval = $interval;
        }

    }
}