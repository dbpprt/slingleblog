/// <reference path="../_references.ts"/>

module Application {

    export interface IPostModel extends ng.resource.IResource<IPostModel> {
        title: string;
        content: string;
        author: string;
        categories: string[];
        pubDate: string;
        numberOfComments: number;
        excerpt: string;
        slug: string;
    }
}

/*
 public string Title { get; set; }

 public string Content { get; set; }

 public string Author { get; set; }

 public string[] Categories { get; set; }

 public DateTime PubDate { get; set; }

 public int NumberOfComments { get; set; }

 public string Excerpt { get; set; }

 public string Slug { get; set; }
 */