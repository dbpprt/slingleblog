/// <reference path="../_references.ts"/> 

module Application {

    var app = angular.module("app");

    app.controller("BlogController",
        ["$scope", "$routeParams", "PostService",
            ($scope, $routeParams : ng.route.IRouteParamsService, postService)
                => new BlogController($scope, postService, $routeParams)]);

    export class BlogController {
        scope: any;
        postService: IPostResource;

        posts: IPostModel[];

        constructor(
            $scope: IHomeControllerScope,
            postService: IPostResource,
            $routeParams : ng.route.IRouteParamsService
            ) {
            this.scope = $scope;
            this.postService = postService;

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
            this.postService.query({page: page, pageSize: 10 }).$promise.then((posts : IPostModel[]) => {
                this.posts = posts;
                //angular.forEach(posts, (post : IPostModel) => {
                //    alert(post.title);
                //})
            });
        }


    }
}