/// <reference path="../_references.ts"/>

module Application {

    var app = angular.module("app");

    app.factory("PostService", ['$resource', ($resource: ng.resource.IResourceService)
        => constructPostService($resource)]);

    export interface IPostParameters {
        slug: string;
    }

    export interface IPostResource extends ng.resource.IResourceClass<IPostModel> {
        get(): IPostModel;
        get(params: IPostParameters, onSuccess: Function): IPostModel;
    }

    export function constructPostService($resource: ng.resource.IResourceService): IPostResource {
        var updateAction: ng.resource.IActionDescriptor = {
            method: 'PUT',
            isArray: false
        };

        /*
        var getAction: ng.resource.IActionDescriptor = {
            method: 'GET',
            isArray: false
        };
        */

        return <IPostResource> $resource('/api/post/:slug', { slug: '@slug' }, {
            update: updateAction
        });
    };
}