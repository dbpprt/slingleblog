/// <reference path="../_references.ts"/>

module Application {

    var app = angular.module("app");

    app.factory("ApiService", ['$http', ($http: ng.http.IHttpService) => new ApiService($http)]);

    export class ApiService
    {
        $http: ng.http.IHttpService;

        constructor(
            $http : ng.http.IHttpService
            ) {
            this.$http = $http;
        }

        posts(page? : int) : IPostModel[] {
            var result : IPostModel[] = [
                new Application.PostModel({ title: "test", "slug": "test", pubDate : "heute", tags : ["some", "tag"], content : "<h1>test</h1>" })
            ];

            return result;
        }

        post(slug : string) : IPostModel {
            return new Application.PostModel({ title: "test", "slug": "test", pubDate : "heute", tags : ["some", "tag"], content : "<h1>test</h1>" })
        }
    }

}