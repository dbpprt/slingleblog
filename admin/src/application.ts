/// <reference path="_references.ts" />

module Application {
    'use strict';

    var app = angular.module("app", ["ngRoute", "ngResource", "ngSanitize", "angular-loading-bar", "ngAnimate"]);

    app.directive('newScope', function () {
        return {
            scope: true,
            priority: 450
        };
    });
    app.config(function (cfpLoadingBarProvider) {
        cfpLoadingBarProvider.includeSpinner = false;
    });

    app.config(function ($routeProvider, $locationProvider) {
        $locationProvider.html5Mode(true);
        $routeProvider
            .when('/', {
                redirectTo: () => {
                    return "/blog/"
                }
            })
            .when('/blog/:page?', {
                templateUrl: 'home/blog.html',
                controller: 'BlogController as vm'
            })
            .when('/article/:slug', {
                templateUrl: 'home/post.html',
                controller: 'PostController as vm'
            })
            .when('/page2', { templateUrl: 'page2.html', controller: 'BlogController' })
    });

}