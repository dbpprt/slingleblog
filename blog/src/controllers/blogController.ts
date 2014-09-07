/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("BlogController",
        ["$scope", "$routeParams", "ApiService",
            ($scope, $routeParams : ng.route.IRouteParamsService, apiService)
                => new BlogController($scope, apiService, $routeParams)]);

    export class BlogController {
        scope: any;
        apiService: ApiService;

        posts: IPostModel[];

        constructor(
            $scope: IHomeControllerScope,
            apiService: ApiService,
            $routeParams : ng.route.IRouteParamsService
            ) {
            this.scope = $scope;
            this.apiService = apiService;

            var page : number = $routeParams["page"];
            if (page && page > 0)
                this.showPosts(page);
            else this.showPosts(1);

            this.scope.$on('$routeChangeSuccess', function (ev, current, prev) {
                console.log("test");
                // ...
            });

        }

        showPosts(page: int) {
            this.posts = this.apiService.posts(page);
            angular.forEach(this.posts, (post : IPostModel) => {
                alert(post.title);
            })
        }


    }
}