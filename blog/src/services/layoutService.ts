/// <reference path="../_references.ts"/>

module Application {

    var app = angular.module("app");

    app.factory("LayoutService", ['$resource', ($resource: ng.resource.IResourceService) => new LayoutService()]);

    export interface ILayoutService {
        toggleAbout: any;
        toggleTags: any;
    }

    export class LayoutService
    {
        aboutActive: boolean;
        tagsActive: boolean;

        constructor() {

        }

        toggleAbout() {
            if (this.tagsActive) {
                this.toggleTags();
            }
            this.aboutActive = !this.aboutActive;
            $('#about-canvas').show();
            $('#activity-canvas').hide();
            $('#site-wrapper').toggleClass("show-about-canvas");
        }

        toggleTags() {
            if (this.aboutActive) {
                this.toggleAbout();
            }
            this.tagsActive = !this.tagsActive;
            $('#activity-canvas').show();
            $('#about-canvas').hide();
            $('#site-wrapper').toggleClass("show-activity-canvas");
        }
    }

}