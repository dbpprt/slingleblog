/// <reference path="../_references.ts"/>

module Application {

    export interface IPostModel {
        title: string;
        content: string;
        tags: string[];
        pubDate: string;
        slug: string;
    }

    export class PostModel implements IPostModel {
        title: string;
        content: string;
        tags: string[];
        pubDate: string;
        slug: string;

        constructor (options?: {title: string; content: string; tags: string[]; pubDate: string; slug:string;}) {
            if (options) {
                this.title = options.title;
                this.content = options.content;
                this.tags = options.tags;
                this.pubDate = options.pubDate;
                this.slug = options.slug;
            }
        }
    }
}