/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("HomeController", ["$scope", "$interval", "UserService", ($scope, $interval, userService: IUserResource)
        => new HomeController($scope, $interval)]);


    export interface IHomeControllerScope extends ng.IScope {
        displayText: string;
        counter: number;
        aList: string[];
    }

    export class HomeController {

        scope: IHomeControllerScope;
        interval: ng.IIntervalService;
        //userService: IUserResource;

        constructor(
            $scope: IHomeControllerScope,
            $interval: ng.IIntervalService
            //userService: IUserResource
            ) {
            this.scope = $scope;
            this.interval = $interval;
            //this.userService = userService;

            $scope.counter = 0;

            $scope.displayText = "hello bro";
            $interval(() => this.increment(), 1000);
            $scope.aList = [];
            $scope.aList.push("hello bro!");


        }

        increment() {
            this.scope.counter++;
            this.scope.aList.push("the counter says " + this.scope.counter);
        }
    }
}