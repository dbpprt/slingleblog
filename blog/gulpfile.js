var gulp = require('gulp');
var config = require('./build.config.js');
var es = require('event-stream');
var request = require('request');
var plugins = require('gulp-load-plugins')();
var fs = require('fs');
var Q = require('q');

var task = { dev: {}, build: {} };

task.clean = function () {
    return gulp.src([config.outputFolder + '/*'], { read: false })
        .pipe(plugins.rimraf());
};

task.assets = function() {
    gulp.src(config.assets)
        .pipe(gulp.dest(config.outputFolder + '/assets/'));
}

task.deploy = function () {
    function callback(error, response, body) {
        if (!error && response.statusCode == 200) {
            console.log(body);
        } else {
            console.log(error);
        }
        return gulp.src('./deploy.zip')
            .pipe(plugins.clean());
    }
    console.log("Starting deploy to " + config.deployRequestOptions.url);
    //request = request.defaults({'proxy':'http://localhost:8888'});
    fs.createReadStream('deploy.zip').pipe(request.put(config.deployRequestOptions, callback));
}

task.package = function () {
    return gulp.src(config.outputFolder + '/**/*')
        .pipe(plugins.zip('deploy.zip'))
        .pipe(gulp.dest('./'));
}

task.styleBundles = function() {
    var promises = [];

    config.styleBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.less({
                compress: true
            }))
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(plugins.minifyCss())
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.dev.scriptBundles = function() {
    var promises = [];

    config.scriptBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.build.scriptBundles = function() {
    var promises = [];

    config.scriptBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.uglify())
            //.pipe(plugins.obfuscate())
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.dev.typescriptBundles = function() {
    var promises = [];

    config.typescriptBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.typescriptCompiler({
                module: '',
                target: 'ES3',
                sourcemap: false,
                logErrors: true
            }))
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.build.typescriptBundles = function() {
    var promises = [];

    config.typescriptBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.typescriptCompiler({
                module: '',
                target: 'ES3',
                sourcemap: false,
                logErrors: true
            }))
            .pipe(plugins.ngmin())
            .pipe(plugins.angularFilesort())
            .pipe(plugins.uglify())
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.templateBundles = function () {
    var promises = [];

    config.templateBundles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.files)
            .pipe(plugins.htmlmin({
                removeComments: true,
                collapseWhitespace: true,
                collapseBooleanAttributes: true,
                removeAttributeQuotes: true,
                removeRedundantAttributes: true,
                useShortDoctype: true,
                removeEmptyAttributes: true
            }))
            .pipe(plugins.angularHtmlify())
            .pipe(plugins.angularTemplatecache({
                module: "app"
            }))
            .pipe(plugins.obfuscate())
            .pipe(plugins.uglify())
            .pipe(plugins.concat(bundle.outputFile))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

task.indexFiles = function () {
    var promises = [];

    config.indexFiles.forEach(function(bundle) {
        var defer = Q.defer();
        var pipeline = gulp.src(bundle.sourceFile)
            .pipe(plugins.inject(gulp.src(bundle.includes, { read: false }), {
                ignorePath: bundle.ignore
            }))
            .pipe(plugins.angularHtmlify())
            .pipe(plugins.htmlmin({
                removeComments: true,
                collapseWhitespace: true,
                collapseBooleanAttributes: true,
                removeAttributeQuotes: true,
                removeRedundantAttributes: true,
                useShortDoctype: true,
                removeEmptyAttributes: true
            }))
            .pipe(gulp.dest(bundle.targetFolder));

        pipeline.on('end', function () {
            defer.resolve();
        });
        promises.push(defer.promise);
    });
    return Q.all(promises);
}

gulp.task('clean', task.clean);

gulp.task('assets', ["clean"], task.assets);

gulp.task('package', ["index-files"], task.package);
gulp.task('deploy', ["package", "index-files"], task.deploy);

gulp.task('template-bundles', ["clean"], task.templateBundles);

gulp.task('style-bundles', ["clean"], task.styleBundles);

if (config.buildConfiguration == "debug") {
    console.log("Building in debug mode")
    gulp.task('typescript-bundles', ["clean"], task.dev.typescriptBundles);
    gulp.task('script-bundles', ["clean", "template-bundles", "typescript-bundles"], task.dev.scriptBundles);

} else {
    console.log("Building in release mode")
    gulp.task('typescript-bundles', ["clean"], task.build.typescriptBundles);
    gulp.task('script-bundles', ["clean", "template-bundles", "typescript-bundles"], task.build.scriptBundles);
}

gulp.task('index-files', ["clean", "assets", "template-bundles", "typescript-bundles", "script-bundles", "style-bundles"], task.indexFiles)

gulp.task('default', ["deploy"]);