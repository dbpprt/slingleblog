var deploySettings = {
    release: {
        authorizationToken : "5X2WbulsDjjfEMgYG21oF3TFU5RIFRvh7MNLNvexyq1y0n1OLE3AHoaBmqLgyKaOAee3egYOIxxtEtuIk4fu8cDLihoGbiazIVNi5HkZYZxexaK2ibvi1vAAzD9ZceD7",
        applicationEndpoint : "http://devapp:8080/api/deploy",
        targetServerFolder : ""
    },
    debug: {
        authorizationToken : "5X2WbulsDjjfEMgYG21oF3TFU5RIFRvh7MNLNvexyq1y0n1OLE3AHoaBmqLgyKaOAee3egYOIxxtEtuIk4fu8cDLihoGbiazIVNi5HkZYZxexaK2ibvi1vAAzD9ZceD7",
        applicationEndpoint : "http://devapp:8080/api/deploy",
        targetServerFolder : ""
    }
}

exports.outputFolder = "build";
exports.sourceFolder = "src";

exports.buildConfiguration = "release";

exports.scriptBundles = [
    {
        outputFile : "vendor.js",
        targetFolder: this.outputFolder + '/assets/scripts',
        files : [
            './bower_components/angular/angular.js',
            './bower_components/angular-route/angular-route.js',
            './bower_components/jquery/dist/jquery.js',
            './bower_components/bootstrap/dist/js/bootstrap.js',
            './bower_components/angular-resource/angular-resource.js',
            './bower_components/angular-animate/angular-animate.js',
            './bower_components/angular-sanitize/angular-sanitize.js',
            './bower_components/angular-loading-bar/build/loading-bar.js',
            './bower_components/angular-bootstrap/ui-bootstrap.js'
        ]
    }
];

exports.styleBundles = [
    {
        outputFile : "base.css",
        targetFolder: this.outputFolder + '/assets/styles',
        files : [
            './bower_components/angular-loading-bar/src/loading-bar.css',
                './' + this.sourceFolder + '/styles/base.less'
        ]
    }
];

exports.templateBundles = [
    {
        outputFile : "templates.js",
        targetFolder: this.outputFolder + '/assets/scripts',
        files : [
            './' + this.sourceFolder + '/views/**/*.html',
            '!./' + this.sourceFolder + '/views/index.html'
        ]
    }
];

exports.typescriptBundles = [
    {
        outputFile : "app.js",
        targetFolder : this.outputFolder + '/assets/scripts',
        files : [
            '!./bower_components/**/*',
                './' + this.sourceFolder + '/**/*.ts/',
            '!./node_modules/**/*'
        ]
    }
];

exports.assets = [
    './' + this.sourceFolder + '/assets/**/*',
    './bower_components/font-awesome/fonts/fontawesome-webfont.eot',
    './bower_components/open-sans-fontface/fonts/Light/OpenSans-Light.eot'
];

exports.indexFiles = [
    {
        sourceFile : './' + this.sourceFolder + '/views/index.html',
        targetFolder : this.outputFolder,

        ignore : [
            '/' + this.outputFolder
        ],

        includes : [
            this.outputFolder + '/assets/styles/*.css',
            this.outputFolder + '/assets/scripts/vendor*.js',
            this.outputFolder + '/assets/scripts/app*.js',
            this.outputFolder + '/assets/scripts/templates*.js'
        ]
    }
]

exports.indexFile = './' + this.sourceFolder + '/views/index.html'

exports.deployRequestOptions = {
    url: deploySettings[this.buildConfiguration].applicationEndpoint,
    headers: {
        'AccessToken': deploySettings[this.buildConfiguration].authorizationToken,
		'TargetFolder': deploySettings[this.buildConfiguration].targetServerFolder
    }
};
