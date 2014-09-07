/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("PostController",
        ["$routeParams", "ApiService",
            ($routeParams : ng.route.IRouteParamsService, apiService)
                => new PostController(apiService, $routeParams)]);

    export class PostController {
        apiService: ApiService;
        post: IPostModel;

        constructor(
            apiService: ApiService,
            $routeParams : ng.route.IRouteParamsService
            ) {
            this.apiService = apiService;

            var slug : string = $routeParams["slug"];
            if (slug ) this.showPost(slug);
        }

        showPost(slug: string) {
            this.post = this.apiService.post(slug);
        }
    }
}