
module Application {

    var app = angular.module("app");

    app.controller("RightbarController", ["LayoutService", (layoutService) => new RightbarController(layoutService)]);

    export class RightbarController {
        layoutService: ILayoutService;
        some: string;

        constructor(
            layoutService: ILayoutService
            ) {

            this.layoutService = layoutService;
            this.some = "gsdkgs";
        }

        onAboutClicked = function() {
            this.layoutService.toggleAbout();
        }

        onTagsClicked() {
            this.layoutService.toggleTags();
        }
    }
}