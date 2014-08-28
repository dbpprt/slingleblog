var gulp = require('gulp');
var config = require('./build.config.js');
var es = require('event-stream');
var request = require('request');
var plugins = require('gulp-load-plugins')();
var fs = require('fs');

var task = { dev: {}, build: {} };

task.clean = function () {
    return gulp.src([config.outputFolder + '/*'], { read: false })
        .pipe(plugins.clean());
};

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

    //request = request.defaults({'proxy':'http://localhost:8888'});
    fs.createReadStream('deploy.zip').pipe(request.put(config.deployRequestOptions, callback));
}

task.package = function () {
    return gulp.src(config.outputFolder + '/**/*')
        .pipe(plugins.zip('deploy.zip'))
        .pipe(gulp.dest('./'));
}

task.assets = function() {
    gulp.src(config.assets)
        .pipe(gulp.dest(config.outputFolder + '/assets/'));
}

task.dev.typescript = function () {
    return gulp.src(config.typescriptFiles)
        .pipe(plugins.typescriptCompiler({
            module: '',
            target: 'ES3',
            sourcemap: false,
            logErrors: true
        }))
        .pipe(gulp.dest(config.outputFolder + '/assets/typescripts'));
};

task.build.typescript = function () {
    return gulp.src(config.typescriptFiles)
        .pipe(plugins.typescriptCompiler({
            module: '',
            target: 'ES3',
            sourcemap: false,
            logErrors: true
        }))
        .pipe(plugins.ngmin())
        .pipe(plugins.angularFilesort())
        .pipe(plugins.uglify())
        .pipe(plugins.obfuscate())
        .pipe(plugins.concat('app.js'))
        .pipe(plugins.rev())
        .pipe(gulp.dest(config.outputFolder + '/assets/scripts'));
};

task.dev.templates = function () {
    return gulp.src(config.templates)
        .pipe(plugins.angularHtmlify())
        .pipe(plugins.angularTemplatecache({
            module: "app"
        }))
        .pipe(gulp.dest(config.outputFolder + '/assets/scripts'));
};

task.templates = function () {
    return gulp.src(config.templates)
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
        .pipe(plugins.concat('templates.js'))
        .pipe(plugins.rev())
        .pipe(gulp.dest(config.outputFolder + '/assets/scripts'));
};

task.dev.styles = function () {
    return gulp.src(config.styleReferences)
    .pipe(plugins.less({
        compress: false
    }))
    .pipe(gulp.dest(config.outputFolder + '/assets/styles'));
};

task.dev.index = function () {
    var files = [
        config.outputFolder + '/assets/styles/*.css',
        config.outputFolder + '/assets/scripts/vendor.js'
    ];

    var angularFiles = [
        config.outputFolder + '/assets/typescripts/**/*.js',
        config.outputFolder + '/assets/scripts/templates.js'
    ];

    return gulp.src(config.indexFile)
        .pipe(
            plugins.inject(
                es.merge(
                    gulp.src(files, { read: false }),
                    gulp.src(angularFiles, { read: false }).pipe(plugins.angularFilesort())
                ), {
                    ignorePath: ['/' + config.outputFolder]
                }
            )
        )
        .pipe(plugins.angularHtmlify())
        .pipe(gulp.dest(config.outputFolder));
};

task.build.scripts = function () {
    return gulp.src(config.scriptRefereces)
        .pipe(plugins.uglify())
        //.pipe(plugins.obfuscate())
        .pipe(plugins.concat('vendor.js'))
        .pipe(plugins.rev())
        .pipe(gulp.dest(config.outputFolder + '/assets/scripts'));
};

task.dev.scripts = function () {
    return gulp.src(config.scriptReferences)
        .pipe(plugins.concat('vendor.js'))
        .pipe(gulp.dest(config.outputFolder + '/assets/scripts'));
}

task.build.styles = function () {
    return gulp.src(config.styleReferences)
        .pipe(plugins.less({
            compress: true
        }))
        .pipe(plugins.concat('all.css'))
        .pipe(plugins.minifyCss())
        .pipe(plugins.rev())
        .pipe(gulp.dest(config.outputFolder + '/assets/styles'));
};

task.build.index = function () {
    var files = [
        config.outputFolder + '/assets/styles/*.css',
        config.outputFolder + '/assets/scripts/vendor*.js',
        config.outputFolder + '/assets/scripts/app*.js',
        config.outputFolder + '/assets/scripts/templates*.js'
    ];

    return gulp.src(config.indexFile)
        .pipe(plugins.inject(gulp.src(files, { read: false }), {
            ignorePath: ['/' + config.outputFolder]
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
        .pipe(gulp.dest(config.outputFolder));
};


gulp.task('clean', task.clean);
gulp.task('package', task.package)
gulp.task('deploy', ['package'], task.deploy);
gulp.task('assets', ['clean'], task.assets);
gulp.task('templates', ['clean'], task.templates);
gulp.task('build:typescript', ['clean'], task.build.typescript);
gulp.task('typescript', ['clean'], task.dev.typescript);
gulp.task('dev:templates', ['clean'], task.dev.templates);
gulp.task('dev:fonts', ['clean'], task.dev.fonts)

gulp.task('build:styles', ['clean'], task.build.styles);
gulp.task('build:scripts', ['clean', 'templates', 'build:typescript'], task.build.scripts);
gulp.task('build:index', ['clean', 'build:styles', 'build:typescript', 'build:scripts'], task.build.index);
gulp.task('build', ['clean', 'build:styles', 'build:typescript', 'build:scripts', 'build:index', 'assets']);

gulp.task('styles', ['clean', 'dev:fonts'], task.dev.styles);
gulp.task('index', ['clean', 'dev:templates', 'styles', 'scripts', 'typescript', 'dev:fonts'], task.dev.index);
gulp.task('scripts', ['clean', 'dev:templates', 'typescript'], task.dev.scripts);
gulp.task('default', ['clean', 'styles', 'scripts', 'dev:templates', 'typescript', 'index', 'assets']);

gulp.task('watch', task.dev.watch);
