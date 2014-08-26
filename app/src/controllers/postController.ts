/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("PostController",
        ["$scope", "$routeParams", "PostService",
            ($scope, $routeParams : ng.route.IRouteParamsService, postService)
                => new PostController($scope, postService, $routeParams)]);

    export class PostController {
        scope: any;
        postService: IPostResource;
        post: IPostModel;

        constructor(
            $scope: IHomeControllerScope,
            postService: IPostResource,
            $routeParams : ng.route.IRouteParamsService
            ) {
            this.scope = $scope;
            this.postService = postService;

            var slug : string = $routeParams["slug"];
            if (slug ) this.showPost(slug);
        }

        showPost(slug: string) {
            this.postService.get({slug: slug}).$promise.then((post : IPostModel) => {
                this.post = post;
            });
        }
    }
}