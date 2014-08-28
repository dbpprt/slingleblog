/// <reference path="../_references.ts"/>

module Application {

    var app = angular.module("app");

    app.factory("UserService", ['$resource', ($resource: ng.resource.IResourceService)
        => constructUserService($resource)]);

    export interface IUserParameters {
        id: number;
    }

    export interface IUser extends ng.resource.IResource<IUser> {
        id: number;
        firstName: string;
        lastName: string;
    }

    export interface IUserResource extends ng.resource.IResourceClass<IUser> {
        get(): IUser;
        get(params: IUserParameters, onSuccess: Function): IUser;
    }

    export function constructUserService($resource: ng.resource.IResourceService): IUserResource {
        var updateAction: ng.resource.IActionDescriptor = {
            method: 'PUT',
            isArray: false
        };

        return <IUserResource> $resource('/api/user/:id', { id: '@id' }, {
            update: updateAction
        });
    };
}